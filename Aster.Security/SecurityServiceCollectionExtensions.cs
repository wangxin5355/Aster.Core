using Aster.Security;
using Aster.Security.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aster.Security
{
    public static class SecurityServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services, Action<TokenOptions> configureTokenOptions)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureTokenOptions == null)
                throw new ArgumentNullException(nameof(configureTokenOptions));

            services.AddOptions();
            services.Configure(configureTokenOptions);

            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<WhiteListFilterAttribute>();

            return services;
        }
    }
}
