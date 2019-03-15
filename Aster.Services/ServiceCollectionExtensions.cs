using Aster.Common.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using System;
using System.Linq;

namespace Aster.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var types = typeof(ServiceCollectionExtensions).Assembly.GetTypes();
            var svcTypes = types
                .Where(x => x.FullName.Contains(".Impl.", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            foreach (var t in svcTypes)
            {
                foreach (var interf in t.GetInterfaces())
                {
                    services.AddScoped(interf, t);
                }
            }

            var singletons = types.Where(x => !x.IsAbstract
                && x.IsClass
                && typeof(ISingleton).IsAssignableFrom(x));

            foreach (var m in singletons)
            {
                foreach (var i in m.GetInterfaces())
                {
                    if (i.IsGenericType)
                    {
                        services.AddSingleton(i.MakeGenericType(), m.MakeGenericType());
                    }
                    else
                    {
                        services.AddSingleton(i, m);
                    }
                }
            }

            services.AddMailKit(optionBuilder =>
            {
                var opts = new MailKitOptions()
                {
                    Server = configuration["EmailSetting:Server"],
                    Port = Convert.ToInt32(configuration["EmailSetting:Port"]),
                    SenderName = configuration["EmailSetting:SenderName"],
                    SenderEmail = configuration["EmailSetting:SenderEmail"],

                    // enable ssl or tls
                    Security = Convert.ToBoolean(configuration["EmailSetting:Security"])
                };
                if (Convert.ToBoolean(configuration["EmailSetting:NeedCredentials"]))
                {
                    opts.Account = configuration["EmailSetting:Account"];
                    opts.Password = configuration["EmailSetting:Password"];
                }
                optionBuilder.UseMailKit(opts);
            });
        }
    }
}
