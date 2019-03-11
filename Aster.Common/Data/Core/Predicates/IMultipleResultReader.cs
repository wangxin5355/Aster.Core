using System.Collections.Generic;

namespace Aster.Common.Data.Core.Predicates
{
    public interface IMultipleResultReader
    {
        IEnumerable<T> Read<T>();
    }
}