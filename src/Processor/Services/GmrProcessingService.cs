using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Extensions;
using FluentValidation;
using FluentValidation.Results;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Services;

public class GmrProcessingService(
    ILogger<GmrProcessingService> logger,
    ITradeImportsDataApiClient api,
    IValidator<DataApiGvms.Gmr> validator
) : IGmrProcessingService
{
    public async Task ProcessGmr(DataApiGvms.Gmr gmr, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(gmr, cancellationToken);
        if (!validationResult.IsValid)
        {
            LogValidationErrors(gmr, validationResult);
            return;
        }

        var gmrId = gmr.Id!;

        logger.LogInformation("Processing Gmr for {GmrId}", gmrId);

        var existingGmr = await api.GetGmr(gmrId, cancellationToken);
        if (existingGmr != null && !ShouldProcess(gmr, existingGmr.Gmr))
        {
            return;
        }

        if (existingGmr == null)
        {
            await api.PutGmr(gmrId, gmr, null, cancellationToken);
            return;
        }

        logger.LogInformation("Updating existing Gmr {GmrId}", gmrId);

        await api.PutGmr(gmrId, gmr, existingGmr.ETag, cancellationToken);
    }

    private void LogValidationErrors(DataApiGvms.Gmr gmr, ValidationResult validationResult)
    {
        validationResult.Errors.ForEach(error =>
            logger.LogInformation(
                "Gmr {GmrId} failed validation with {ErrorCode}: {ErrorMessage}",
                gmr.Id,
                error.CustomState ?? error.ErrorCode,
                error.ErrorMessage
            )
        );
    }

    private bool ShouldProcess(DataApiGvms.Gmr newGmr, DataApiGvms.Gmr existingGmr)
    {
        if (NewGmrIsOlderThanExistingGmr(newGmr, existingGmr))
        {
            logger.LogInformation(
                "Skipping {GmrId} because new Gmr is older: {NewTime:O} < {OldTime:O}",
                newGmr.Id,
                newGmr.UpdatedSource,
                existingGmr.UpdatedSource
            );

            return false;
        }

        return true;
    }

    private static bool NewGmrIsOlderThanExistingGmr(DataApiGvms.Gmr newGmr, DataApiGvms.Gmr existingGmr)
    {
        return newGmr.UpdatedSource.TrimMicroseconds() <= existingGmr.UpdatedSource.TrimMicroseconds();
    }
}
