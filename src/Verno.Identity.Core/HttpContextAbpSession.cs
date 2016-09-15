using System.Threading;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;

namespace Verno.Identity
{
    /// <summary>
    /// Implements IAbpSession to get session informations from ASP.NET Identity framework.
    /// </summary>
    public class HttpContextAbpSession : ClaimsAbpSession, ISingletonDependency
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public override long? UserId
        {
            get
            {
                //string userId = Thread.CurrentPrincipal.Identity.GetUserId();
                string userId = _contextAccessor.HttpContext.User.Identity.GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return new long?();
                long result;
                if (!long.TryParse(userId, out result))
                    return new long?();
                return result;
            }
        }

        /// <summary>Constructor.</summary>
        public HttpContextAbpSession(IMultiTenancyConfig multiTenancy, IHttpContextAccessor contextAccessor)
          : base(multiTenancy)
        {
            _contextAccessor = contextAccessor;
        }
    }
}