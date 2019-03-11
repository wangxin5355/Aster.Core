using Dapper;
using Aster.Common.Data.Builder;
using Aster.Common.Data.Core;
using Aster.Common.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aster.Common.Data.Repositories
{
    public partial class DapperRepository<T> : IRepository<T> where T : class, IEntity
    {
        public virtual async Task<T> GetAsync(int id)
        {
            var session = GetSession();
            return await _dapperImplementor.GetAsync<T>(session, id, session.Transaction, null);
        }

        public virtual async Task<T> GetAsync(long id)
        {
            var session = GetSession();
            return await _dapperImplementor.GetAsync<T>(session, id, session.Transaction, null);
        }

        public virtual async Task<T> GetAsync(Guid id)
        {
            var session = GetSession();
            return await _dapperImplementor.GetAsync<T>(session, id, session.Transaction, null);
        }

        public virtual async Task<dynamic> InsertAsync(T item)
        {
            var session = GetSession();
            return await _dapperImplementor.InsertAsync(session, item, session.Transaction, null);
        }

        public virtual async Task InsertAsync(IEnumerable<T> items)
        {
            var session = GetSession();
            await _dapperImplementor.InsertAsync(session, items, session.Transaction, null);
        }

        public virtual async Task<bool> UpdateAsync(T item)
        {
            var session = GetSession();
            return await _dapperImplementor.UpdateAsync(session, item, session.Transaction, null);
        }

        public virtual async Task<bool> DeleteAsync(T item)
        {
            var session = GetSession();
            return await _dapperImplementor.DeleteAsync(session, item, session.Transaction, null);
        }

        public virtual async Task<IList<T>> GetListAsync()
        {
            var session = GetSession();
            return (await _dapperImplementor.GetListAsync<T>(session, null, null, session.Transaction, null, null, false)).ToList();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            var session = GetSession();
            return await _dapperImplementor.CountAsync<T>(session, QueryBuilder<T>.FromExpression(predicate), session.Transaction, null);
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate = null)
        {
            var session = GetSession();
            return await _dapperImplementor.DeleteAsync<T>(session, QueryBuilder<T>.FromExpression(predicate), session.Transaction, null);
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(string sql, object param = null, int? timeout = null)
        {
            var session = GetSession();
            return await session.QueryAsync<T>(sql, param, session.Transaction, timeout);
        }

        public virtual async Task<IEnumerable<dynamic>> QueryDynamicAsync(string sql, object param = null, int? timeout = null)
        {
            var session = GetSession();
            return await session.QueryAsync<dynamic>(sql, param, session.Transaction, timeout);
        }

        public async Task<object> QueryScalarAsync(string sql, object param = null, int? timeout = null)
        {
            var session = GetSession();
            return await session.ExecuteScalarAsync(sql, param, session.Transaction, timeout);
        }
    }
}
