using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aster.Common.Data.Core.Enums;
using Aster.Common.Data.Core.Predicates;
using Aster.Common.Data.Core.Sql;
using Aster.Common.Data.Sql;

namespace Aster.Common.Data.Predicates
{
    /// <summary>
    /// Groups IPredicates together using the specified group operator.
    /// </summary>
    public class PredicateGroup : IPredicateGroup
    {
        public GroupOperator Operator { get; set; }
        public IList<IPredicate> Predicates { get; set; }
        public string GetSql(ISqlGenerator sqlGenerator, IDictionary<string, object> parameters)
        {
            if (Predicates == null || Predicates.Count == 0)
                return "1=1";

            string seperator = Operator == GroupOperator.And ? " AND " : " OR ";
            return "(" + Predicates.Aggregate(new StringBuilder(),
                (sb, p) => (sb.Length == 0 ? sb : sb.Append(seperator)).Append(p.GetSql(sqlGenerator, parameters)),
                sb =>
                {
                    var s = sb.ToString();
                    if (s.Length == 0) return sqlGenerator.Configuration.Dialect.EmptyExpression; 
                    return s;
                }
                ) + ")";
        }
    }
}