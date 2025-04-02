using Defra.TradeImportsProcessor.Api.Domain;

namespace Defra.TradeImportsProcessor.Api.Services;

public interface IGmrService
{
    Task<Gmr?> GetGmr(string gmrId);
}
