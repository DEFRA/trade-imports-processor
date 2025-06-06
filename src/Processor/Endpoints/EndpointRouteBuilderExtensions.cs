using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Amazon.SQS.Model;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Microsoft.AspNetCore.Mvc;
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
}
