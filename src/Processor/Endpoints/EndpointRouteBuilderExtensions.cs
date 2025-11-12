using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Data;
using Defra.TradeImportsProcessor.Processor.Data.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

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
