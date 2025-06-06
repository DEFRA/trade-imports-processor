using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
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
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await notificationConsumer.OnHandle(body, cancellationToken);
        }
        catch (HttpRequestException httpRequestException)
            when (httpRequestException.StatusCode == HttpStatusCode.Conflict)
        {
            var logger = loggerFactory.CreateLogger(nameof(ReplayImportPreNotifications));
            logger.LogWarning(httpRequestException, "409 Conflict");

            return Results.Conflict();
        }

        return Results.Accepted();
    }
}
