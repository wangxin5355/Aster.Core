using Aster.Common.Data.Core.Predicates;

namespace Aster.Common.Data.Predicates
{
    public interface IBetweenPredicate : IPredicate
    {
        string PropertyName { get; set; }
        BetweenValues Value { get; set; }
        bool Not { get; set; }

    }
}