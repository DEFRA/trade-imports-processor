namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DictionaryMapper
{
    public static IDictionary<string, object> Map(IDictionary<string, object>? from)
    {
        if (from == null)
        {
            return null!;
        }

        var dic = new Dictionary<string, object>();
        foreach (var item in from)
        {
            dic.Add(item.Key, item.Value);
        }

        return dic;
    }
}
