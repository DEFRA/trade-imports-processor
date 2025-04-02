using Defra.TradeImportsProcessor.Api.Domain;

namespace Defra.TradeImportsProcessor.Api.Services;

public class GmrService : IGmrService
{
    public Task<Gmr?> GetGmr(string gmrId) => Task.FromResult<Gmr?>(null);
}
