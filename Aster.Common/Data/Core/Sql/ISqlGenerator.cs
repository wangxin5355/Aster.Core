using System.Collections.Generic;
using Aster.Common.Data.Core.Configuration;
using Aster.Common.Data.Core.Mapper;
using Aster.Common.Data.Core.Predicates;

namespace Aster.Common.Data.Core.Sql
{
    public interface ISqlGenerator
    {
        IDapperConfiguration Configuration { get; }
        
        string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters);
        string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDictionary<string, object> parameters);
        string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters);
        string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);
        string Insert(IClassMapper classMap);
        string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);
        string Delete(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);
        string IdentitySql(IClassMapper classMap);
        string GetTableName(IClassMapper map);
        string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias);
        string GetColumnName(IClassMapper map, string propertyName, bool includeAlias);
        string BuildSelectColumns(IClassMapper classMap);
        bool SupportsMultipleStatements();
    }
}