using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Services;

public interface IGmrProcessingService
{
    Task ProcessGmr(DataApiGvms.Gmr gmr, CancellationToken cancellationToken);
}
