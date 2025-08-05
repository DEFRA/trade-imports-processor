using System.Linq.Expressions;

namespace Defra.TradeImportsProcessor.Processor.Data;

public interface IFieldUpdateBuilder<T>
{
    IFieldUpdateBuilder<T> Set<TField>(Expression<Func<T, TField>> field, TField value);
}
