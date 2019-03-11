using System;
using System.Collections.Generic;
using Aster.Common.Data.Core.Predicates;

namespace Aster.Common.Data.Predicates
{
    public class GetMultiplePredicateItem : IGetMultiplePredicateItem
    {
        public object Value { get; set; }
        public Type Type { get; set; }
        public IList<ISort> Sort { get; set; }
    }
}