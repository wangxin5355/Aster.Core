using Aster.Common.Data.Core.Enums;

namespace Aster.Common.Data.Core.Predicates
{
    public interface IComparePredicate : IBasePredicate
    {
        Operator Operator { get; set; }
        bool Not { get; set; }
    }
}