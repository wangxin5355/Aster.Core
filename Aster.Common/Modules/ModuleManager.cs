using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Aster.Common.Modules
{
    public static class ModuleManager
    {
        private static IList<Assembly> _allModules;

        public static IList<Assembly> GetAllModules()
        {
            if (_allModules == null)
            {
                var dllFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _allModules = Directory.GetFiles(dllFolder, "Aster*.dll")
                 .Select(x => Assembly.LoadFrom(x))
                 .Where(x => x.GetCustomAttribute(typeof(ModuleAttribute)) != null)
                 .ToList();
            }

            return _allModules;
        }

        /// <summary>
        /// 自动注册各个模块中的：INavigationProvider,IPermissionProvider,Services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddModules(this IServiceCollection services)
        {
            var types = GetAllModules().SelectMany(x => x.GetTypes());

            //var adminMeunus = types.Where(x => !x.IsAbstract && x.IsClass && typeof(INavigationProvider).IsAssignableFrom(x));
            //foreach (var m in adminMeunus)
            //{
            //    services.AddSingleton(typeof(INavigationProvider), m);
            //}
            //var permissions = types.Where(x => !x.IsAbstract && x.IsClass && typeof(IPermissionProvider).IsAssignableFrom(x));
            //foreach (var m in permissions)
            //{
            //    services.AddSingleton(typeof(IPermissionProvider), m);
            //}

            var svrs = types.Where(x => !x.IsAbstract
                && x.IsClass
                && x.Namespace != null
                && x.Namespace.Contains("Impl", StringComparison.InvariantCultureIgnoreCase));

            foreach (var m in svrs)
            {
                foreach (var i in m.GetInterfaces())
                {
                    if (i.Namespace != null && i.Namespace.StartsWith("Aster", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (i.IsGenericType)
                        {
                            services.AddScoped(i.MakeGenericType(), m.MakeGenericType());
                        }
                        else
                        {
                            services.AddScoped(i, m);
                        }
                    }
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

            return services;
        }
    }
}
