namespace Aster.Common.Data.Core.Predicates
{
    public interface IFieldPredicate : IComparePredicate
    {
        object Value { get; set; }
    }
}