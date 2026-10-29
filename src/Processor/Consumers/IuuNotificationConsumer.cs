using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Models.Iuu;
using FluentValidation;
using SlimMessageBus;
using DataApiIuu = Defra.TradeImportsDataApi.Domain.Iuu;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class IuuNotificationConsumer(
    ILogger<IuuNotificationConsumer> logger,
    ITradeImportsDataApiClient api,
    IValidator<IuuNotification> validator
) : IConsumer<IuuNotification>
{
    public async Task OnHandle(IuuNotification received, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(received, cancellationToken);
        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(error =>
                logger.LogInformation(
                    "IUU Notification {IuuReference} failed validation with {ErrorCode}: {ErrorMessage}",
                    received.Reference,
                    error.CustomState ?? error.ErrorCode,
                    error.ErrorMessage
                )
            );

            throw new ArgumentException($"IUU Notification {received.Reference} failed validation");
        }

        var iuuReference = received.Reference;

        logger.LogInformation("Received IUU Notification {IuuReference}", iuuReference);

        var existingIuuNotification = await api.GetIuuNotification(iuuReference, cancellationToken);

        if (existingIuuNotification == null)
        {
            await CreateNewIuuNotification(received, cancellationToken);
            return;
        }

        if (received.LastUpdated > existingIuuNotification.IuuNotification.LastUpdated)
        {
            await UpdateExistingIuuNotification(received, existingIuuNotification.ETag, cancellationToken);
            return;
        }

        if (received.LastUpdated <= existingIuuNotification.IuuNotification.LastUpdated)
        {
            logger.LogInformation(
                "Skipping IUU Notification {IuuReference} as the last updated timestamp of the message received {NewIuuTime:O} is older than, or the same as, the existing IUU Notification record's last updated timestamp {ExistingIuuTime:O}.",
                iuuReference,
                received.LastUpdated,
                existingIuuNotification.IuuNotification.LastUpdated
            );
        }
    }

    private async Task CreateNewIuuNotification(IuuNotification received, CancellationToken cancellationToken)
    {
        var mappedIuu = (DataApiIuu.IuuNotification)received;

        await api.PutIuuNotification(received.Reference, mappedIuu, null, cancellationToken);
    }

    private async Task UpdateExistingIuuNotification(
        IuuNotification received,
        string? existingEtag,
        CancellationToken cancellationToken
    )
    {
        var mappedIuu = (DataApiIuu.IuuNotification)received;

        await api.PutIuuNotification(received.Reference, mappedIuu, existingEtag, cancellationToken);
    }
}
