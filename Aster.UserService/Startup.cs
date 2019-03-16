using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aster.Common.Filter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Aster.UserService.Extensions;
using Aster.Cache;
using Aster.Security;
using Aster.Services;
using Aster.Common.Data;
using Aster.Localizations;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace Aster.UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedRedisCache(Configuration);
            services.AddLocalizationOption(opts => Configuration.GetSection("Localization").Bind(opts));
            services.AddSqlStringLocalizer(opts => Configuration.GetSection("Localization").Bind(opts));

            //security
            services.AddSecurity((opts) => Configuration.GetSection("TokenOptions").Bind(opts));
            services.AddServices(Configuration);
            services.AddData(Configuration);
            services.AddConsul(Configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("user", new Info { Title = "UserServiceApi", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Aster.UserService.xml");
                c.IncludeXmlComments(xmlPath);
            });
            services.AddMvcCore().AddApiExplorer();
            services.AddMvc(options => {
                options.Filters.Add(typeof(AuthorizationFilter));
                options.Filters.Add(typeof(MyExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }  
            app.UseConsul();
            app.UseSecurity();
            app.UseMvc();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/Sapi/user/swagger.json", "user");
            });
        }
    }
}
