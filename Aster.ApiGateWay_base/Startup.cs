using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Extensions.PlatformAbstractions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Swashbuckle.AspNetCore.Swagger;

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
         
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "GW", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Aster.ApiGateWay_base.xml");
                c.IncludeXmlComments(xmlPath);
            });
            services.AddOcelot(Configuration).AddPolly().AddConsul().AddConfigStoredInConsul();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc().UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/user/swagger.json", "UserServiceApi");
                c.SwaggerEndpoint("/api/trade/swagger.json", "TradeServiceApi");
            });
            app.UseHsts();
            //app.UseMiddleware<ApiRequestLogMiddlerware>();
            app.UseOcelot().Wait();
        }
    }
}
