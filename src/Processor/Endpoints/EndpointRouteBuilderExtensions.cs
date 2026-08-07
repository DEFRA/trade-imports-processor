using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Authentication;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Data;
using Defra.TradeImportsProcessor.Processor.Data.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MongoDB.Driver.Linq;
using Trade.Gateway.Api.Contract.Certificate;
using Trade.Gateway.Api.Contract.Events;

namespace Defra.TradeImportsProcessor.Processor.Endpoints;

[ExcludeFromCodeCoverage]
public static class EndpointRouteBuilderExtensions
{
    public static void MapRawMessageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("raw-messages", GetByFilter).RequireAuthorization();
        app.MapGet("raw-messages/{messageId}", Get).RequireAuthorization();
        app.MapGet("raw-messages/{messageId}/json", GetJson).RequireAuthorization();
    }

    public static void MapDevEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("dev/traces-cheds", PostTracesChed).RequireAuthorization(PolicyNames.Execute);
        app.MapPost("dev/matched-gmrs", PostMatchedGmr).RequireAuthorization(PolicyNames.Execute);
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

    [HttpPost]
    private static async Task<IResult> PostTracesChed(
        HttpRequest request,
        [FromServices] IOptions<TraceHeader> traceHeader,
        [FromServices] ITraceContextAccessor traceContextAccessor,
        [FromServices] HeaderPropagationValues headerPropagationValues,
        [FromServices] TracesChedConsumer consumer,
        CancellationToken cancellationToken
    )
    {
        var ched = await request.ReadFromJsonAsync<DefraUNVTDCHEDProfile>(cancellationToken);
        if (ched is null)
            return Results.BadRequest("Request body could not be deserialized as a TRACES CHED.");

        var traceId =
            request.Headers.TryGetValue(traceHeader.Value.Name, out var headerValue)
            && !StringValues.IsNullOrEmpty(headerValue)
                ? headerValue.ToString().Replace("-", "")
                : Guid.NewGuid().ToString("N");

        traceContextAccessor.Context = new TraceContext { TraceId = traceId };

        var headers = headerPropagationValues.Headers ??= new Dictionary<string, StringValues>(
            StringComparer.OrdinalIgnoreCase
        );

        headers[traceHeader.Value.Name] = traceId;

        await consumer.OnHandle(ched.ToEventEnvelope(traceId), cancellationToken);
        return Results.NoContent();
    }

    [HttpPost]
    private static async Task<IResult> PostMatchedGmr(
        HttpRequest request,
        [FromServices] IOptions<TraceHeader> traceHeader,
        [FromServices] ITraceContextAccessor traceContextAccessor,
        [FromServices] HeaderPropagationValues headerPropagationValues,
        [FromServices] MatchedGmrConsumer consumer,
        CancellationToken cancellationToken
    )
    {
        var matchedGmr = await request.ReadFromJsonAsync<MatchedGmr>(cancellationToken);
        if (matchedGmr is null)
            return Results.BadRequest("Request body could not be deserialized as a MatchedGmr.");

        var traceId =
            request.Headers.TryGetValue(traceHeader.Value.Name, out var headerValue)
            && !StringValues.IsNullOrEmpty(headerValue)
                ? headerValue.ToString().Replace("-", "")
                : Guid.NewGuid().ToString("N");

        traceContextAccessor.Context = new TraceContext { TraceId = traceId };

        var headers = headerPropagationValues.Headers ??= new Dictionary<string, StringValues>(
            StringComparer.OrdinalIgnoreCase
        );

        headers[traceHeader.Value.Name] = traceId;

        await consumer.OnHandle(matchedGmr, cancellationToken);
        return Results.NoContent();
    }
}
