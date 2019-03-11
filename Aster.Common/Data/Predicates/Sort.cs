using Aster.Common.Data.Core.Predicates;

namespace Aster.Common.Data.Predicates
{
    public class Sort : ISort
    {
        public string PropertyName { get; set; }
        public bool Ascending { get; set; }
    }
}