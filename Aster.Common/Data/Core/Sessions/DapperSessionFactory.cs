using Aster.Common.Data.Core.Attributes;
using Aster.Common.Data.Core.Configuration;
using Aster.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Aster.Common.Data.Core.Sessions
{
    public class DapperSessionFactory : IDapperSessionFactory
    {
        private readonly ILogger<DapperSessionFactory> _logger;
        private readonly IDapperConfiguration _configuration;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly Lazy<ConcurrentDictionary<Type, string>> _loadedContexts;

        public DapperSessionFactory(IDapperConfiguration configuration, ILogger<DapperSessionFactory> logger, IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
            _configuration = configuration;
            _logger = logger;
            _loadedContexts = new Lazy<ConcurrentDictionary<Type, string>>(() => GetAllLoadedContexts());
        }

        public IDapperSession GetSession(string connectionStringName)
        {
            if (string.IsNullOrWhiteSpace(connectionStringName))
                throw new ArgumentNullException(nameof(connectionStringName));

            var connection = _configuration.Dialect.GetConnection(_connectionStringProvider.ConnectionString(connectionStringName));

            _logger.LogDebug($"get session for {connectionStringName}");

            return new DapperSession(connection);
        }

        public string GetTypeConnectionStringName(Type type)
        {
            if (!_loadedContexts.Value.TryGetValue(type, out string conStringName))
                throw new MyException($"{type.FullName}不是有效的IEntity类型");

            return conStringName;
        }


        private ConcurrentDictionary<Type, string> GetAllLoadedContexts()
        {
            var dic = _configuration
                .Assemblies
                .SelectMany(a => a.GetTypes().Where(t => t.IsClass && typeof(IEntity).IsAssignableFrom(t)))
                .ToDictionary(t => t, t =>
                {
                    var attribute = t.GetCustomAttribute<DataContextAttribute>(true);
                    if (attribute == null)
                        return _configuration.DefaultConnectionStringName;
                    return attribute.ConnectionStringName;
                });
            return new ConcurrentDictionary<Type, string>(dic);
        }
    }
}
