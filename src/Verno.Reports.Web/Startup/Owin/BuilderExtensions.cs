using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Owin.Builder;
using Microsoft.Owin.BuilderProperties;
using Owin;
using Microsoft.AspNetCore.DataProtection;

namespace Verno.Reports.Web.Startup.Owin
{
    /// <summary>
    /// This class (UseAppBuilder method) integrates OWIN pipeline to ASP.NET Core pipeline and
    /// allows us to use Owin based middlewares in ASP.NET Core applications.
    /// </summary>
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseAppBuilder(
            this IApplicationBuilder app,
            Action<IAppBuilder> configure)
        {
            app.UseOwin(addToPipeline =>
            {
                addToPipeline(next =>
                {
                    var appBuilder = new AppBuilder();
                    var lifetime = (IApplicationLifetime)app.ApplicationServices.GetService(typeof(IApplicationLifetime));

                    var properties = new AppProperties(appBuilder.Properties);
                    properties.AppName = app.ApplicationServices.GetApplicationUniqueIdentifier();
                    properties.OnAppDisposing = lifetime.ApplicationStopping;
                    properties.DefaultApp = next;

                    configure(appBuilder);

                    return appBuilder.Build<Func<IDictionary<string, object>, Task>>();
                });
            });

            return app;
        }
    }
}