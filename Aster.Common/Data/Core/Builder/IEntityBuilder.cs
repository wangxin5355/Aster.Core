using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aster.Common.Data.Core.Builder
{
    public interface IEntityBuilder<T> where T : class, IEntity
    {
        bool Any();
        IEnumerable<T> AsEnumerable();
        IList<T> ToList();
        int Count();
        T Single();
        T SingleOrDefault();
        T FirstOrDefault();
        IEntityBuilder<T> Take(int number);
        IEntityBuilder<T> Offset(int offset);
        IEntityBuilder<T> OrderBy(Expression<Func<T, object>> expression);
        IEntityBuilder<T> OrderByDescending(Expression<Func<T, object>> expression);

        /// <summary>
        /// SqlCe cannot have a non zero timeout.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        IEntityBuilder<T> Timeout(int timeout);
        IEntityBuilder<T> Nolock();

        #region async
        Task<bool> AnyAsync();
        Task<IEnumerable<T>> AsEnumerableAsync();
        Task<IList<T>> ToListAsync();
        Task<int> CountAsync();
        Task<T> SingleAsync();
        Task<T> SingleOrDefaultAsync();
        Task<T> FirstOrDefaultAsync();
        #endregion
    }
}