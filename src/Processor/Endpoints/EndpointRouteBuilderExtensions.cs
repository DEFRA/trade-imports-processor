using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Microsoft.AspNetCore.Mvc;
using SlimMessageBus;

namespace Defra.TradeImportsProcessor.Processor.Endpoints;

[ExcludeFromCodeCoverage]
public static class EndpointRouteBuilderExtensions
{
    public static void MapReplayEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("replay/import-pre-notifications", ReplayImportPreNotifications).RequireAuthorization();
    }

    [HttpPost]
    private static async Task<IResult> ReplayImportPreNotifications(
        HttpRequest request,
        [FromServices] IConsumer<JsonElement> notificationConsumer,
        [FromBody] JsonElement body,
        CancellationToken cancellationToken
    )
    {
        await notificationConsumer.OnHandle(body, cancellationToken);

        return Results.Accepted();
    }
}
