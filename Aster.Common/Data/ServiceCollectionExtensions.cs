using Aster.Common.Data.Core;
using Aster.Common.Data.Core.Configuration;
using Aster.Common.Data.Core.Implementor;
using Aster.Common.Data.Core.Mapper;
using Aster.Common.Data.Core.Repositories;
using Aster.Common.Data.Core.Sessions;
using Aster.Common.Data.Core.Sql;
using Aster.Common.Data.Implementor;
using Aster.Common.Data.Mapper;
using Aster.Common.Data.MySql;
using Aster.Common.Data.Repositories;
using Aster.Common.Data.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Aster.Common.Data
{
    public static class ServiceCollectionExtensions
    {
        public static void AddData(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            IDapperConfiguration getConfiguration(ILoggerFactory loggerFactory) =>
                 DapperConfiguration
                 .Use(GetAllConnectionStrings(configuration), loggerFactory)
                 .UseClassMapper(typeof(AutoClassMapper<>))
                 .UseSqlDialect(new MySqlDialect())
                 .WithDefaultConnectionStringNamed("DefaultConnectionString")
                 .FromAssemblies(GetEntityAssemblies())
                 .Build();

            services.AddSingleton(x => getConfiguration(x.GetRequiredService<ILoggerFactory>()));
            services.AddSingleton<IConnectionStringProvider, StaticConnectionStringProvider>();
            services.AddSingleton<IDapperSessionFactory, DapperSessionFactory>();
            services.AddScoped<IDapperSessionContext, DapperSessionContext>();
            services.AddScoped<ISqlGenerator, SqlGeneratorImpl>();
            services.AddScoped<IDapperImplementor, DapperImplementor>();
            services.AddScoped(typeof(IRepository<>), typeof(DapperRepository<>));
        }

        private static IDictionary<string, string> GetAllConnectionStrings(IConfiguration configuration)
        {
            var sections = configuration.GetSection("ConnectionStrings");

            return sections.GetChildren().ToDictionary(x => x.Key, x => x.Value);
        }

        private static IEnumerable<Assembly> GetEntityAssemblies()
        {
            var dllFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Directory.GetFiles(dllFolder, "Aster.*.dll")
                 .SelectMany(x => Assembly.LoadFrom(x).GetTypes())
                 .Where(x => typeof(IEntity).IsAssignableFrom(x))
                 .Select(x => x.Assembly)
                 .Distinct();
        }
    }
}
