using System.Collections.Generic;
using Aster.Common.Data.Core.Enums;

namespace Aster.Common.Data.Core.Predicates
{
    public interface IPredicateGroup : IPredicate
    {
        GroupOperator Operator { get; set; }
        IList<IPredicate> Predicates { get; set; }
    }
}