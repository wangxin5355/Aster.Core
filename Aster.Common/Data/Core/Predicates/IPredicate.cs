using System.Collections.Generic;
using Aster.Common.Data.Core.Sql;

namespace Aster.Common.Data.Core.Predicates
{
    public interface IPredicate
    {
        string GetSql(ISqlGenerator sqlGenerator, IDictionary<string, object> parameters);
    }
}