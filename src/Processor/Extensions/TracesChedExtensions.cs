using System.Globalization;
using Trade.Gateway.Api.Contract.Certificate;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class TracesChedExtensions
{
    private const string LastUpdateSubjectCode = "LAST_UPDATE_DATETIME";

    public static DateTimeOffset? GetLatestLastUpdateDateTime(this DefraUNVTDCHEDProfile ched)
    {
        var notes = ched.ExchangedDocument.IncludedNote;
        if (notes == null)
        {
            return null;
        }

        return notes
            .Where(n => string.Equals(n.SubjectCode?.Value, LastUpdateSubjectCode, StringComparison.OrdinalIgnoreCase))
            .SelectMany(n => n.Content ?? Enumerable.Empty<string>())
            .Select(c =>
            {
                if (DateTimeOffset.TryParse(c, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
                {
                    return (DateTimeOffset?)dto;
                }

                return null;
            })
            .Where(d => d.HasValue)
            .Max();
    }
}
