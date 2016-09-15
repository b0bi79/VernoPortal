using System;
using System.Net;
using System.Threading.Tasks;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Verno.Identity;
using Verno.Identity.Roles;
using Verno.Identity.Users;
using Verno.Reports.EntityFrameworkCore;
using Verno.Reports.Web.Configuration;
using Verno.Reports.Web.Modules.Print;

namespace Verno.Reports.Web.Startup
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            env.ApplicationName = "Verno.Portal";
            Configuration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ReportsDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Default"))
            );

            services.AddDbContext<PrintDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Print"))
            );

            services.AddDbContext<Identity.Data.IdentityDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Identity"))
            );

            // добавление сервисов Idenity
            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = User.MinimumPasswordLength;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                //.AddEntityFrameworkStores()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            //Configure Abp and Dependency Injection
            var result = services.AddAbp<ReportsWebModule>(options =>
            {
                options.ConfigureIdentity("Reports");

                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                );
            });
            return result;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new DbLoggerProvider());

            app.UseAbp(); //Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("~/Error?statusCode={0}");
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
