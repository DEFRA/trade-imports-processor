using Defra.TradeImportsProcessor.Processor.Authentication;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Endpoints.Admin;

public static class EndpointRouteBuilderExtensions
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("admin/resource-events/dlq/redrive", Redrive)
            .WithName(nameof(Redrive))
            .WithTags("Admin")
            .WithSummary("Initiates redrive of messages from the dead letter queue")
            .WithDescription("Redrives all messages on the resource events dead letter queue")
            .Produces(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/resource-events/dlq/remove-message", RemoveMessage)
            .WithName(nameof(RemoveMessage))
            .WithTags("Admin")
            .WithSummary("Initiates removal of message from the dead letter queue")
            .WithDescription(
                "Attempts to find and remove a message on the resource events dead letter queue by message ID"
            )
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/resource-events/dlq/drain", Drain)
            .WithName(nameof(Drain))
            .WithTags("Admin")
            .WithSummary("Initiates drain of all messages from the dead letter queue")
            .WithDescription("Drains all messages on the resource events dead letter queue")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/customs-declarations/dlq/redrive", Redrive)
            .WithName(nameof(RedriveCustomsDeclarations))
            .WithTags("Admin")
            .WithSummary("Initiates redrive of messages from the dead letter queue")
            .WithDescription("Redrives all messages on the resource events dead letter queue")
            .Produces(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/customs-declarations/dlq/remove-message", RemoveMessage)
            .WithName(nameof(RemoveCustomsDeclarationsMessage))
            .WithTags("Admin")
            .WithSummary("Initiates removal of message from the dead letter queue")
            .WithDescription(
                "Attempts to find and remove a message on the resource events dead letter queue by message ID"
            )
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/customs-declarations/dlq/drain", Drain)
            .WithName(nameof(DrainCustomsDeclarations))
            .WithTags("Admin")
            .WithSummary("Initiates drain of all messages from the dead letter queue")
            .WithDescription("Drains all messages on the resource events dead letter queue")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);
    }

    [HttpPost]
    private static async Task<IResult> Redrive(
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<ResourceEventsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (
                !await sqsDeadLetterService.Redrive(
                    options.Value.DeadLetterQueueName,
                    options.Value.QueueName,
                    cancellationToken
                )
            )
            {
                return Results.InternalServerError();
            }
        }
        catch (Exception)
        {
            return Results.InternalServerError();
        }

        return Results.Accepted();
    }

    [HttpPost]
    private static async Task<IResult> RemoveMessage(
        string messageId,
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<ResourceEventsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await sqsDeadLetterService.Remove(
                messageId,
                options.Value.DeadLetterQueueName,
                cancellationToken
            );

            return Results.Content(result, "text/plain; charset=utf-8");
        }
        catch (Exception)
        {
            return Results.InternalServerError();
        }
    }

    [HttpPost]
    private static async Task<IResult> Drain(
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<ResourceEventsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (!await sqsDeadLetterService.Drain(options.Value.DeadLetterQueueName, cancellationToken))
            {
                return Results.InternalServerError();
            }
        }
        catch (Exception)
        {
            return Results.InternalServerError();
        }

        return Results.Ok();
    }

    [HttpPost]
    private static async Task<IResult> RedriveCustomsDeclarations(
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<CustomsDeclarationsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (
                !await sqsDeadLetterService.Redrive(
                    options.Value.DeadLetterQueueName,
                    options.Value.QueueName,
                    cancellationToken
                )
            )
            {
                return Results.InternalServerError();
            }
        }
        catch (Exception)
        {
            return Results.InternalServerError();
        }

        return Results.Accepted();
    }

    [HttpPost]
    private static async Task<IResult> RemoveCustomsDeclarationsMessage(
        string messageId,
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<CustomsDeclarationsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await sqsDeadLetterService.Remove(
                messageId,
                options.Value.DeadLetterQueueName,
                cancellationToken
            );

            return Results.Content(result, "text/plain; charset=utf-8");
        }
        catch (Exception)
        {
            return Results.InternalServerError();
        }
    }

    [HttpPost]
    private static async Task<IResult> DrainCustomsDeclarations(
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<CustomsDeclarationsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (!await sqsDeadLetterService.Drain(options.Value.DeadLetterQueueName, cancellationToken))
            {
                return Results.InternalServerError();
            }
        }
        catch (Exception)
        {
            return Results.InternalServerError();
        }

        return Results.Ok();
    }
}
