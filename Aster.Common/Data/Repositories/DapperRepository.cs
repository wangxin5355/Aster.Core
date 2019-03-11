using Dapper;
using Aster.Common.Data.Builder;
using Aster.Common.Data.Core;
using Aster.Common.Data.Core.Builder;
using Aster.Common.Data.Core.Implementor;
using Aster.Common.Data.Core.Repositories;
using Aster.Common.Data.Core.Sessions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aster.Common.Data.Repositories
{
    public partial class DapperRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly IDapperImplementor _dapperImplementor;
        protected readonly IDapperSessionContext _sessionContext;

        public DapperRepository(IDapperSessionContext sessionContext, IDapperImplementor dapperImplementor)
        {
            _dapperImplementor = dapperImplementor;
            _sessionContext = sessionContext;
        }

        public virtual T Get(int id)
        {
            var session = GetSession();
            return _dapperImplementor.Get<T>(session, id, session.Transaction, null);
        }

        public virtual T Get(long id)
        {
            var session = GetSession();
            return _dapperImplementor.Get<T>(session, id, session.Transaction, null);
        }

        public virtual T Get(Guid id)
        {
            var session = GetSession();
            return _dapperImplementor.Get<T>(session, id, session.Transaction, null);
        }

        public virtual dynamic Insert(T item)
        {
            var session = GetSession();
            return _dapperImplementor.Insert(session, item, session.Transaction, null);
        }

        public virtual void Insert(IEnumerable<T> items)
        {
            var session = GetSession();
            _dapperImplementor.Insert(session, items, session.Transaction, null);
        }

        public virtual bool Update(T item)
        {
            var session = GetSession();
            return _dapperImplementor.Update(session, item, session.Transaction, null);
        }

        public virtual bool Delete(T item)
        {
            var session = GetSession();
            return _dapperImplementor.Delete(session, item, session.Transaction, null);
        }

        public virtual IList<T> GetList()
        {
            var session = GetSession();
            return _dapperImplementor.GetList<T>(session, null, null, session.Transaction, null, false, null, false).ToList();
        }

        public virtual IEntityBuilder<T> Query(Expression<Func<T, bool>> predicate)
        {
            var session = GetSession();
            return new EntityBuilder<T>(session, predicate, _dapperImplementor);
        }

        public virtual int Count(Expression<Func<T, bool>> predicate = null)
        {
            var session = GetSession();
            return _dapperImplementor.Count<T>(session, QueryBuilder<T>.FromExpression(predicate), session.Transaction, null);
        }

        public virtual bool Delete(Expression<Func<T, bool>> predicate = null)
        {
            var session = GetSession();
            return _dapperImplementor.Delete<T>(session, QueryBuilder<T>.FromExpression(predicate), session.Transaction, null);
        }

        public virtual IEnumerable<T> Query(string sql, object param = null, int? timeout = null)
        {
            var session = GetSession();
            return session.Query<T>(sql, param, session.Transaction, true, timeout);
        }

        public virtual IEnumerable<dynamic> QueryDynamic(string sql, object param = null, int? timeout = null)
        {
            var session = GetSession();
            return session.Query<dynamic>(sql, param, session.Transaction, true, timeout);
        }

        public object QueryScalar(string sql, object param = null, int? timeout = null)
        {
            var session = GetSession();
            return session.ExecuteScalar(sql, param, session.Transaction, timeout);
        }

        public IDapperSession GetSession()
        {
            return _sessionContext.GetSession<T>();
        }

        public async Task<int> Execute(string sql, object param = null, int? timeout = null, CommandType? commandType = null)
        {
            var session = GetSession();

            return await session.ExecuteAsync(sql, param, session.Transaction, timeout, commandType);
        }
    }
}
