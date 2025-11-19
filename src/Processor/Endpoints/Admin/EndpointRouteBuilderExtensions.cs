using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Authentication;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Endpoints.Admin;

[ExcludeFromCodeCoverage(Justification = "This is covered by integration tests")]
public static class EndpointRouteBuilderExtensions
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("admin/resource-events/dlq/redrive", RedriveResourceEvents)
            .WithName(nameof(RedriveResourceEvents))
            .WithTags("Admin")
            .WithSummary("Initiates redrive of messages from the dead letter queue")
            .WithDescription("Redrives all messages on the resource events dead letter queue")
            .Produces(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/resource-events/dlq/remove-message", RemoveResourceEventsMessage)
            .WithName(nameof(RemoveResourceEventsMessage))
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

        app.MapPost("admin/resource-events/dlq/drain", DrainResourceEvents)
            .WithName(nameof(DrainResourceEvents))
            .WithTags("Admin")
            .WithSummary("Initiates drain of all messages from the dead letter queue")
            .WithDescription("Drains all messages on the resource events dead letter queue")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/customs-declarations/dlq/redrive", RedriveCustomsDeclarations)
            .WithName(nameof(RedriveCustomsDeclarations))
            .WithTags("Admin")
            .WithSummary("Initiates redrive of messages from the Customs Declarations dead letter queue")
            .WithDescription("Redrives all messages on the Customs Declarations dead letter queue")
            .Produces(StatusCodes.Status202Accepted)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/customs-declarations/dlq/remove-message", RemoveCustomsDeclarationsMessage)
            .WithName(nameof(RemoveCustomsDeclarationsMessage))
            .WithTags("Admin")
            .WithSummary("Initiates removal of message from the Customs Declarations dead letter queue")
            .WithDescription(
                "Attempts to find and remove a message on the Customs Declarations dead letter queue by message ID"
            )
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);

        app.MapPost("admin/customs-declarations/dlq/drain", DrainCustomsDeclarations)
            .WithName(nameof(DrainCustomsDeclarations))
            .WithTags("Admin")
            .WithSummary("Initiates drain of all messages from the Customs Declarations dead letter queue")
            .WithDescription("Drains all messages on the Customs Declarations dead letter queue")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status405MethodNotAllowed)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(PolicyNames.Execute);
    }

    [HttpPost]
    private static Task<IResult> RedriveResourceEvents(
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<ResourceEventsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        return Redrive(
            sqsDeadLetterService,
            options.Value.DeadLetterQueueName,
            options.Value.QueueName,
            cancellationToken
        );
    }

    [HttpPost]
    private static Task<IResult> RemoveResourceEventsMessage(
        string messageId,
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<ResourceEventsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        return RemoveMessage(messageId, sqsDeadLetterService, options.Value.DeadLetterQueueName, cancellationToken);
    }

    [HttpPost]
    private static Task<IResult> DrainResourceEvents(
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<ResourceEventsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        return Drain(sqsDeadLetterService, options.Value.DeadLetterQueueName, cancellationToken);
    }

    [HttpPost]
    private static Task<IResult> RedriveCustomsDeclarations(
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<CustomsDeclarationsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        return Redrive(
            sqsDeadLetterService,
            options.Value.DeadLetterQueueName,
            options.Value.QueueName,
            cancellationToken
        );
    }

    [HttpPost]
    private static Task<IResult> RemoveCustomsDeclarationsMessage(
        string messageId,
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<CustomsDeclarationsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        return RemoveMessage(messageId, sqsDeadLetterService, options.Value.DeadLetterQueueName, cancellationToken);
    }

    [HttpPost]
    private static Task<IResult> DrainCustomsDeclarations(
        [FromServices] ISqsDeadLetterService sqsDeadLetterService,
        [FromServices] IOptions<CustomsDeclarationsConsumerOptions> options,
        CancellationToken cancellationToken
    )
    {
        return Drain(sqsDeadLetterService, options.Value.DeadLetterQueueName, cancellationToken);
    }

    [HttpPost]
    private static async Task<IResult> Redrive(
        ISqsDeadLetterService sqsDeadLetterService,
        string sourceQueue,
        string destinationQueue,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (!await sqsDeadLetterService.Redrive(sourceQueue, destinationQueue, cancellationToken))
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
        ISqsDeadLetterService sqsDeadLetterService,
        string queue,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await sqsDeadLetterService.Remove(messageId, queue, cancellationToken);

            return Results.Content(result, "text/plain; charset=utf-8");
        }
        catch (Exception)
        {
            return Results.InternalServerError();
        }
    }

    [HttpPost]
    private static async Task<IResult> Drain(
        ISqsDeadLetterService sqsDeadLetterService,
        string queue,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (!await sqsDeadLetterService.Drain(queue, cancellationToken))
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
