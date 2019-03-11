using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Aster.Cache
{
    /// <summary>
    /// Extension methods for setting up Redis distributed cache related services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class RedisCacheServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="sentinelConnectionString"></param>
        /// <param name="serverName"></param>
        /// <param name="keyPreffix"></param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var redisOptions = new RedisOptions();
            var config = configuration.GetSection("Redis");

            config.Bind(redisOptions);

            if (redisOptions.Servers == null || redisOptions.Servers.Length == 0) throw new ArgumentException("redis server配置不能为空");

            services.Configure<RedisOptions>(x => config.Bind(x));

            RedisHelper.Initialization(new CSRedisClient(redisOptions.Servers.First()));

            services.Add(ServiceDescriptor.Singleton<IDistributedCache>(x => new CSRedisCache(x.GetRequiredService<IOptions<RedisOptions>>().Value)));

            return services;
        }
    }
}