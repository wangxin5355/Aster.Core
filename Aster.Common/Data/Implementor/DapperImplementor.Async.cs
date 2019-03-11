using Dapper;
using Aster.Common.Data.Core.Enums;
using Aster.Common.Data.Core.Mapper;
using Aster.Common.Data.Core.Predicates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Aster.Common.Data.Implementor
{
    public partial class DapperImplementor
    {
        public async Task<T> GetAsync<T>(IDbConnection connection, dynamic id, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetIdPredicate(classMap, id);
            T result = (await GetListAsync<T>(connection, classMap, predicate, null, transaction, commandTimeout, 1, false)).SingleOrDefault();
            return result;
        }

        public async Task InsertAsync<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            List<IPropertyMap> properties = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey).ToList();

            foreach (T e in entities)
            {
                foreach (IPropertyMap column in properties)
                {
                    if (column.KeyType == KeyType.Guid)
                    {
                        Guid comb = SqlGenerator.Configuration.GetNextGuid();
                        column.PropertyInfo.SetValue(e, comb, null);
                    }
                }
            }

            string sql = SqlGenerator.Insert(classMap);

            await connection.ExecuteAsync(sql, entities, transaction, commandTimeout, CommandType.Text);
        }

        public async Task<dynamic> InsertAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            List<IPropertyMap> nonIdentityKeyProperties = classMap.Properties.Where(p => p.KeyType == KeyType.Guid || p.KeyType == KeyType.Assigned).ToList();
            IPropertyMap identityColumn = classMap.Properties.SingleOrDefault(p => p.KeyType == KeyType.Identity);

            foreach (IPropertyMap column in nonIdentityKeyProperties)
            {
                if (column.KeyType == KeyType.Guid)
                {
                    Guid comb = SqlGenerator.Configuration.GetNextGuid();
                    column.PropertyInfo.SetValue(entity, comb, null);
                }
            }

            IDictionary<string, object> keyValues = new ExpandoObject();
            string sql = SqlGenerator.Insert(classMap);
            if (identityColumn != null)
            {
                IEnumerable<long> result;
                if (SqlGenerator.SupportsMultipleStatements())
                {
                    sql += SqlGenerator.Configuration.Dialect.BatchSeperator + SqlGenerator.IdentitySql(classMap);
                    result = await connection.QueryAsync<long>(sql, entity, transaction, commandTimeout, CommandType.Text);
                }
                else
                {
                    await connection.ExecuteAsync(sql, entity, transaction, commandTimeout, CommandType.Text);
                    sql = SqlGenerator.IdentitySql(classMap);
                    result = await connection.QueryAsync<long>(sql, entity, transaction, commandTimeout, CommandType.Text);
                }

                long identityValue = result.First();
                int identityInt = Convert.ToInt32(identityValue);
                keyValues.Add(identityColumn.Name, identityInt);
                identityColumn.PropertyInfo.SetValue(entity, identityInt, null);
            }
            else
            {
                await connection.ExecuteAsync(sql, entity, transaction, commandTimeout, CommandType.Text);
            }

            foreach (var column in nonIdentityKeyProperties)
            {
                keyValues.Add(column.Name, column.PropertyInfo.GetValue(entity, null));
            }

            if (keyValues.Count == 1)
            {
                return keyValues.First().Value;
            }

            return keyValues;
        }

        public async Task<bool> UpdateAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate(classMap, entity);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Update(classMap, predicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();

            List<IPropertyMap> columns = classMap.Properties
                .Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity))
                .ToList();

            foreach (var property in ReflectionHelper.GetObjectValues(entity, columns))
            {
                DbType type = columns.Where(column => column.Name == property.Key).Select(column => column.Type).First();
                dynamicParameters.Add(property.Key, property.Value, type);
            }

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await connection.ExecuteAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        public async Task<bool> DeleteAsync<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate(classMap, entity);
            return await DeleteAsync(connection, classMap, predicate, transaction, commandTimeout);
        }

        public async Task<bool> DeleteAsync<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return await DeleteAsync(connection, classMap, wherePredicate, transaction, commandTimeout);
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, int? topRecords, bool nolock) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return await GetListAsync<T>(connection, classMap, wherePredicate, sort, transaction, commandTimeout, topRecords, nolock);
        }

        public async Task<IEnumerable<T>> GetListPagedAsync<T>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, int offset, int limit, bool nolock) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return await GetListPagedAsync<T>(connection, classMap, wherePredicate, sort, transaction, commandTimeout, offset, limit, nolock);
        }

        public async Task<int> CountAsync<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Count(classMap, wherePredicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return (int)(await connection.QueryAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text))
                .Single().Total;
        }

        private async Task<IEnumerable<T>> GetListAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, int? topRecords, bool nolock) where T : class
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Select(classMap, predicate, sort, parameters);

            if (topRecords.HasValue)
                sql = SqlGenerator.Configuration.Dialect.SelectLimit(sql, topRecords.Value);

            if (nolock)
                sql = SqlGenerator.Configuration.Dialect.SetNolock(sql);

            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }

        private async Task<IEnumerable<T>> GetListPagedAsync<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, int offset, int limit, bool nolock) where T : class
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SelectSet(classMap, predicate, sort, offset, limit, parameters);

            if (nolock)
                sql = SqlGenerator.Configuration.Dialect.SetNolock(sql);

            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return await connection.QueryAsync<T>(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
        }

        private async Task<bool> DeleteAsync(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Delete(classMap, predicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            return (await connection.ExecuteAsync(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text)) > 0;
        }
    }
}
