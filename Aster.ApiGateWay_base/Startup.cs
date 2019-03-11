using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aster.Cache;
using Aster.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.ConfigEditor;
using Ocelot.ConfigEditor.Security;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
namespace Aster.ApiGateWay_base
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDistributedRedisCache(Configuration);
            ////security
            //services.AddSecurity((opts) => Configuration.GetSection("TokenOptions").Bind(opts));
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddOcelot().AddConsul().AddConfigStoredInConsul();
            services.AddOcelotConfigEditor();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

             app.UseHsts();
            //app.UseMiddleware<ApiRequestLogMiddlerware>();
             app.UseOcelotConfigEditor();
             app.UseOcelot().Wait();
        }
    }
}
