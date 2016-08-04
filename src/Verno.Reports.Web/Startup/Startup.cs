using System;
using Abp.AspNetCore;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Abp.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Auditing;
using Abp.Auditing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Verno.Identity;
using Verno.Identity.Data;
using Verno.Identity.Roles;
using Verno.Identity.Users;
using Verno.Reports.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Verno.Reports.Web.Startup
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ReportsDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Default"))
            );

            services.AddDbContext<Verno.Identity.Data.IdentityDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Identity"))
            );

            // добавление сервисов Idenity
            services.AddIdentity<User, Role>()
                //.AddEntityFrameworkStores()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                options.AddAbp(services); //Add ABP infrastructure to MVC
            });

            //Configure Abp and Dependency Injection
            var result = services.AddAbp<ReportsWebModule>(options =>
            {
                options.ConfigureIdentity("Reports");

                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseLog4Net().WithConfig("log4net.config")
                );
            });
            return result;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); //Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
