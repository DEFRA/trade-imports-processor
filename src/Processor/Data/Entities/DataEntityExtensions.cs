namespace Defra.TradeImportsProcessor.Processor.Data.Entities;

public static class DataEntityExtensions
{
    public static string DataEntityName(this Type type) => type.Name.Replace("Entity", "");
}
