using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;
using Microsoft.Extensions.Logging;
using SlimMessageBus;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class NotificationConsumer(ILogger<NotificationConsumer> logger) : IConsumer<ImportNotification>
{
    public Task OnHandle(ImportNotification from, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification: {Message}", JsonSerializer.Serialize(from));

        var to = new IpaffsDataApi.ImportNotification
        {
            IpaffsId = from?.IpaffsId,
            Etag = from?.Etag,
            ExternalReferences = from?.ExternalReferences?.Select(x => ExternalReferenceMapper.Map(x)).ToArray(),
            ReferenceNumber = from?.ReferenceNumber,
            Version = from?.Version,
            UpdatedSource = from?.LastUpdated,
            LastUpdatedBy = UserInformationMapper.Map(from?.LastUpdatedBy),
            ImportNotificationType = ImportNotificationTypeEnumMapper.Map(from?.ImportNotificationType),
            Replaces = from?.Replaces,
            ReplacedBy = from?.ReplacedBy,
            Status = ImportNotificationStatusEnumMapper.Map(from?.Status),
            SplitConsignment = SplitConsignmentMapper.Map(from?.SplitConsignment),
            ChildNotification = from?.ChildNotification,
            JourneyRiskCategorisation = JourneyRiskCategorisationResultMapper.Map(from?.JourneyRiskCategorisation),
            IsHighRiskEuImport = from?.IsHighRiskEuImport,
            PartOne = PartOneMapper.Map(from?.PartOne),
            DecisionBy = UserInformationMapper.Map(from?.DecisionBy),
            DecisionDate = from?.DecisionDate,
            PartTwo = PartTwoMapper.Map(from?.PartTwo),
            PartThree = PartThreeMapper.Map(from?.PartThree),
            OfficialVeterinarian = from?.OfficialVeterinarian,
            ConsignmentValidations = from?.ConsignmentValidations?.Select(ValidationMessageCodeMapper.Map).ToArray(),
            AgencyOrganisationId = from?.AgencyOrganisationId,
            RiskDecisionLockedOn = from?.RiskDecisionLockedOn,
            IsRiskDecisionLocked = from?.IsRiskDecisionLocked,
            IsBulkUploadInProgress = from?.IsBulkUploadInProgress,
            RequestId = from?.RequestId,
            IsCdsFullMatched = from?.IsCdsFullMatched,
            ChedTypeVersion = from?.ChedTypeVersion,
            IsGMRMatched = from?.IsGMRMatched,
        };

        logger.LogInformation("Mapped notification: {Message}", JsonSerializer.Serialize(to));

        return Task.CompletedTask;
    }
}
