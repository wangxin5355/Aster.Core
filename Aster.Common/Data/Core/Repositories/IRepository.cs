using Aster.Common.Data.Core.Builder;
using Aster.Common.Data.Core.Sessions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aster.Common.Data.Core.Repositories
{
    public interface IRepository<T> where T : class, IEntity
    {
        T Get(Guid id);
        T Get(long id);
        T Get(int id);
        dynamic Insert(T item);
        void Insert(IEnumerable<T> items);
        bool Update(T item);
        bool Delete(T item);
        IList<T> GetList();
        int Count(Expression<Func<T, bool>> predicate = null);
        bool Delete(Expression<Func<T, bool>> predicate = null);
        IEnumerable<T> Query(string sql, object param = null, int? timeout = null);
        IEnumerable<dynamic> QueryDynamic(string sql, object param = null, int? timeout = null);
        object QueryScalar(string sql, object param = null, int? timeout = null);
        IEntityBuilder<T> Query(Expression<Func<T, bool>> predicate = null);
        IDapperSession GetSession();
        Task<int> Execute(string sql, object param = null, int? timeout = null, CommandType? commandType = null);

        #region async
        Task<T> GetAsync(Guid id);
        Task<T> GetAsync(long id);
        Task<T> GetAsync(int id);
        Task<dynamic> InsertAsync(T item);
        Task InsertAsync(IEnumerable<T> items);
        Task<bool> UpdateAsync(T item);
        Task<bool> DeleteAsync(T item);
        Task<IList<T>> GetListAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate = null);
        Task<IEnumerable<T>> QueryAsync(string sql, object param = null, int? timeout = null);
        Task<IEnumerable<dynamic>> QueryDynamicAsync(string sql, object param = null, int? timeout = null);
        Task<object> QueryScalarAsync(string sql, object param = null, int? timeout = null);
        #endregion
    }
}
