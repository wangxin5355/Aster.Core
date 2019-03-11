using Aster.Common.Data.Core;
using Aster.Common.Data.Core.Builder;
using Aster.Common.Data.Core.Implementor;
using Aster.Common.Data.Core.Predicates;
using Aster.Common.Data.Core.Sessions;
using Aster.Common.Data.Predicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Aster.Common.Data.Builder
{
    sealed partial class EntityBuilder<T> : IEntityBuilder<T> where T : class, IEntity
    {
        private readonly IDapperImplementor _dapperImplementor;
        private readonly IDapperSession _session;
        private readonly Expression<Func<T, bool>> _expression;
        private readonly IList<ISort> _sort;
        private int? _take;
        private int? _timeout;
        private bool _nolock;
        private int? _offset;

        public EntityBuilder(IDapperSession session, Expression<Func<T, bool>> expression, IDapperImplementor dapperImplementor)
        {
            _dapperImplementor = dapperImplementor;
            _session = session;
            _expression = expression;
            _sort = new List<ISort>();
        }

        private IEnumerable<T> ResolveEnities()
        {
            IPredicateGroup predicate = QueryBuilder<T>.FromExpression(_expression);

            //IPredicateGroup p = predicate?.Predicates == null ? null : predicate;

            if (_offset.HasValue && _take.HasValue)
                return _dapperImplementor.GetListPaged<T>(_session, predicate, _sort, _session.Transaction, _timeout, false, _offset.Value, _take.Value, _nolock);
            return _dapperImplementor.GetList<T>(_session, predicate, _sort, _session.Transaction, _timeout, false, _take, _nolock);
        }

        public IEnumerable<T> AsEnumerable()
        {
            return ResolveEnities();
        }

        public bool Any()
        {
            return Count() > 0;
        }

        public IList<T> ToList()
        {
            return ResolveEnities().ToList();
        }

        public int Count()
        {
            IPredicateGroup predicate = QueryBuilder<T>.FromExpression(_expression);
            return _dapperImplementor.Count<T>(_session, predicate, _session.Transaction, _timeout);
        }

        public T Single()
        {
            return ResolveEnities().Single();
        }

        public T SingleOrDefault()
        {
            return ResolveEnities().SingleOrDefault();
        }

        public T FirstOrDefault()
        {
            return ResolveEnities().FirstOrDefault();
        }

        public IEntityBuilder<T> OrderBy(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            if (propertyInfo == null) return this;

            var sort = new Sort
            {
                PropertyName = propertyInfo.Name,
                Ascending = true
            };
            _sort.Add(sort);

            return this;
        }

        public IEntityBuilder<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            if (propertyInfo == null) return this;

            var sort = new Sort
            {
                PropertyName = propertyInfo.Name,
                Ascending = false
            };
            _sort.Add(sort);

            return this;
        }

        public IEntityBuilder<T> Take(int number)
        {
            _take = number;
            return this;
        }

        public IEntityBuilder<T> Offset(int offset)
        {
            _offset = offset;
            return this;
        }

        /// <summary>
        /// Timeouts cannot be specified for SqlCe, these will remain zero.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IEntityBuilder<T> Timeout(int timeout)
        {
            _timeout = timeout;
            return this;
        }

        public IEntityBuilder<T> Nolock()
        {
            _nolock = true;
            return this;
        }
    }
}