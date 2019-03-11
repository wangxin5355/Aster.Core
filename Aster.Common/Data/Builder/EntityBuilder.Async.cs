using Aster.Common.Data.Core;
using Aster.Common.Data.Core.Builder;
using Aster.Common.Data.Core.Predicates;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aster.Common.Data.Builder
{
    sealed partial class EntityBuilder<T> : IEntityBuilder<T> where T : class, IEntity
    {
        private async Task<IEnumerable<T>> ResolveEnitiesAsync()
        {
            IPredicateGroup predicate = QueryBuilder<T>.FromExpression(_expression);

            if (_offset.HasValue && _take.HasValue)
                return await _dapperImplementor.GetListPagedAsync<T>(_session, predicate, _sort, _session.Transaction, _timeout, _offset.Value, _take.Value, _nolock);
            return await _dapperImplementor.GetListAsync<T>(_session, predicate, _sort, _session.Transaction, _timeout, _take, _nolock);
        }

        public async Task<IEnumerable<T>> AsEnumerableAsync()
        {
            return await ResolveEnitiesAsync();
        }

        public async Task<bool> AnyAsync()
        {
            return (await CountAsync()) > 0;
        }

        public async Task<IList<T>> ToListAsync()
        {
            return (await ResolveEnitiesAsync()).ToList();
        }

        public async Task<int> CountAsync()
        {
            IPredicateGroup predicate = QueryBuilder<T>.FromExpression(_expression);
            return await _dapperImplementor.CountAsync<T>(_session, predicate, _session.Transaction, _timeout);
        }

        public async Task<T> SingleAsync()
        {
            return (await ResolveEnitiesAsync()).Single();
        }

        public async Task<T> SingleOrDefaultAsync()
        {
            return (await ResolveEnitiesAsync()).SingleOrDefault();
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            return (await ResolveEnitiesAsync()).FirstOrDefault();
        }
    }
}
