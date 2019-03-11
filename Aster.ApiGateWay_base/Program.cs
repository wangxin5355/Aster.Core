using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Aster.ApiGateWay_base
{
    public class Program
    {

        public static void Main(string[] args)
        {
           CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                  WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configBuilder) =>
                {
                    configBuilder
                    .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("ocelot.json")
                    .AddJsonFile("appsettings.json");
                })
                .UseStartup<Startup>();
    }
}
