using System;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.Owin;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Owin;
using Verno.Portal.Web.Modules.Shop;

namespace Verno.Portal.Web.Startup
{
    using Verno.Configuration;
    using Identity;
    using Identity.Roles;
    using Identity.Users;
    using Configuration;
    using Modules.Print;
    using Modules.Returns;
    using Owin;

    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            /*services.AddAbpDbContext<EntityFrameworkCore.ReportsDbContext>(options =>
                EntityFrameworkCore.DbContextOptionsConfigurer.Configure(options.DbContextOptions, options.ConnectionString)
            );*/
            services.AddDbContext<Reports.EntityFrameworkCore.ReportsDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Reports"))
            );

            services.AddDbContext<PrintDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Print"))
            );

            services.AddDbContext<ShopDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Shop"))
            );

            services.AddDbContext<ReturnsDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Returns"))
            );

            services.AddDbContext<Identity.Data.IdentityDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("Identity"))
            );

            /*services.AddAbpDbContext<Identity.Data.IdentityDbContext>(options =>
                Identity.Data.DbContextOptionsConfigurer.Configure(options.DbContextOptions, options.ConnectionString)
            );*/

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

            services.AddOptions();
            services.Configure<AppSettings>(s =>
            {
                s.SiteTitle = "Verno.Portal";
                s.PrintFilesPath = Configuration.GetSection("Print")["FilesPath"];
                s.PrintTemplatesPath = Configuration.GetSection("Common")["TemplatesPath"];
            });

            services.AddMvc(options =>
            {
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            //Configure Abp and Dependency Injection
            var result = services.AddAbp<PortalWebModule>(options =>
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

            app.Properties["host.AppMode"] = "development";
            app.UseDeveloperExceptionPage();

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

            //Integrate to OWIN
            app.UseAppBuilder(ConfigureOwinServices);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void ConfigureOwinServices(IAppBuilder app)
        {
            app.UseAbp();

            app.MapSignalR();

            //Enable it to use HangFire dashboard (uncomment only if it's enabled in AbpProjectNameWebModule)
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new AbpHangfireAuthorizationFilter(AppPermissions.Pages_Administration_HangfireDashboard) }
            //});
        }
    }
}
