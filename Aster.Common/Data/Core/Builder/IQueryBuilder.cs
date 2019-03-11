using System;
using System.Linq.Expressions;
using Aster.Common.Data.Core.Predicates;

namespace Aster.Common.Data.Core.Builder
{
    public interface IQueryBuilder<T> where T : class, IEntity
    {
        IQueryBuilder<T> Equals<TValue>(Expression<Func<T, TValue>> expression, TValue value);
        IQueryBuilder<T> NotEquals<TValue>(Expression<Func<T, TValue>> expression, TValue value);
        IQueryBuilder<T> LessThan<TValue>(Expression<Func<T, TValue>> expression, TValue value);
        IQueryBuilder<T> GreaterThan<TValue>(Expression<Func<T, TValue>> expression, TValue value);
        IQueryBuilder<T> In<TValue>(Expression<Func<T, TValue>> expression, params TValue[] values);
        IQueryBuilder<T> Like<TValue>(Expression<Func<T, TValue>> expression, TValue likeValue);

        IQueryBuilder<T> SubQuery(IQueryBuilder<T> queryBuilder);
        IPredicateGroup GetPredicate();
    }
}