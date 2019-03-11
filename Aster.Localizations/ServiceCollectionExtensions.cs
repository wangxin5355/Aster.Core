using Aster.Localizations.DbStringLocalizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using System;

namespace Aster.Localizations
{
    public static class ServiceCollectionExtensions
    {
        public static void AddLocalizationOption(this IServiceCollection services, Action<LocationOption> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);
        }

        public static void AddSqlStringLocalizer(this IServiceCollection services, Action<SqlLocalizationOptions> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.Configure(configure);

            services.TryAddSingleton(typeof(LocalizationModelContext), typeof(LocalizationModelContext));
            services.TryAddSingleton(typeof(IStringExtendedLocalizerFactory), typeof(SqlStringLocalizerFactory));
            services.TryAddSingleton(typeof(IStringLocalizerFactory), typeof(SqlStringLocalizerFactory));
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        }
    }
}
