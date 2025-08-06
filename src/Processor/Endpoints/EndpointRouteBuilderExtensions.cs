using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Amazon.SQS.Model;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Data;
using Defra.TradeImportsProcessor.Processor.Data.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;
using SlimMessageBus;
using SlimMessageBus.Host;

namespace Defra.TradeImportsProcessor.Processor.Endpoints;

[ExcludeFromCodeCoverage]
public static class EndpointRouteBuilderExtensions
{
    public static void MapReplayEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("replay/import-pre-notifications", ReplayImportPreNotifications).RequireAuthorization();
        app.MapPost("replay/clearance-requests", ReplayClearanceRequests).RequireAuthorization();
        app.MapPost("replay/finalisations", ReplayFinalisations).RequireAuthorization();
    }

    [HttpPost]
    private static async Task<IResult> ReplayImportPreNotifications(
        HttpRequest request,
        [FromServices] NotificationConsumer consumer,
        [FromBody] JsonElement body,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    ) => await HandleMessage(consumer, body, loggerFactory, cancellationToken);

    [HttpPost]
    private static async Task<IResult> ReplayClearanceRequests(
        HttpRequest request,
        [FromServices] CustomsDeclarationsConsumer consumer,
        [FromBody] JsonElement body,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        consumer.Context = CreateContext(InboundHmrcMessageType.ClearanceRequest);

        return await HandleMessage(consumer, body, loggerFactory, cancellationToken);
    }

    [HttpPost]
    private static async Task<IResult> ReplayFinalisations(
        HttpRequest request,
        [FromServices] CustomsDeclarationsConsumer consumer,
        [FromBody] JsonElement body,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        consumer.Context = CreateContext(InboundHmrcMessageType.Finalisation);

        return await HandleMessage(consumer, body, loggerFactory, cancellationToken);
    }

    private static async Task<IResult> HandleMessage(
        IConsumer<JsonElement> consumer,
        JsonElement body,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await consumer.OnHandle(body, cancellationToken);
        }
        catch (HttpRequestException httpRequestException)
            when (httpRequestException.StatusCode == HttpStatusCode.Conflict)
        {
            var logger = loggerFactory.CreateLogger(nameof(HandleMessage));
            logger.LogWarning(httpRequestException, "409 Conflict");

            return Results.Conflict();
        }

        return Results.Accepted();
    }

    private static ConsumerContext CreateContext(string messageType)
    {
        return new ConsumerContext
        {
            Headers = new Dictionary<string, object> { { "InboundHmrcMessageType", messageType } }.AsReadOnly(),
            Properties =
            {
                new KeyValuePair<string, object>("Sqs_Message", new Message { MessageId = Guid.NewGuid().ToString() }),
            },
        };
    }

    public static void MapRawMessageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("raw-messages", GetByFilter).RequireAuthorization();
        app.MapGet("raw-messages/{messageId}", Get).RequireAuthorization();
        app.MapGet("raw-messages/{messageId}/json", GetJson).RequireAuthorization();
    }

    [HttpGet]
    private static async Task<IResult> GetByFilter(
        [FromQuery] string? resourceId,
        [FromQuery] string? messageId,
        [FromServices] IDbContext dbContext,
        [FromServices] IOptions<RawMessageLoggingOptions> options,
        CancellationToken cancellationToken
    )
    {
        if (!options.Value.Enabled)
            return Results.NotFound();

        var query = from entity in dbContext.RawMessages select entity;
        var filtered = false;

        if (!string.IsNullOrWhiteSpace(resourceId))
        {
            query = from entity in query where entity.ResourceId == resourceId select entity;
            filtered = true;
        }

        if (!string.IsNullOrWhiteSpace(messageId))
        {
            query = from entity in query where entity.MessageId == messageId select entity;
            filtered = true;
        }

        if (!filtered)
            return Results.BadRequest("At least one filter param must be specified");

        var results = await query
            .OrderBy(x => x.Updated)
            .Take(100)
            .ToListWithFallbackAsync(cancellationToken: cancellationToken);

        return Results.Ok(results);
    }

    [HttpGet]
    private static async Task<IResult> Get(
        [FromRoute] string messageId,
        [FromServices] IDbContext dbContext,
        [FromServices] IOptions<RawMessageLoggingOptions> options,
        CancellationToken cancellationToken
    )
    {
        if (!options.Value.Enabled)
            return Results.NotFound();

        var entity = await dbContext.RawMessages.FirstOrDefaultAsync(
            x => x.MessageId == messageId,
            cancellationToken: cancellationToken
        );

        return entity is not null ? Results.Ok(entity) : Results.NotFound();
    }

    [HttpGet]
    private static async Task<IResult> GetJson(
        [FromRoute] string messageId,
        [FromServices] IDbContext dbContext,
        [FromServices] IOptions<RawMessageLoggingOptions> options,
        CancellationToken cancellationToken
    )
    {
        if (!options.Value.Enabled)
            return Results.NotFound();

        var entity = await dbContext.RawMessages.FirstOrDefaultAsync(
            x => x.MessageId == messageId,
            cancellationToken: cancellationToken
        );

        return entity is not null ? Results.Content(entity.Message, "application/json") : Results.NotFound();
    }
}
