using Abp.AspNetCore.Mvc.Controllers;

namespace Verno.Portal.Web.Controllers
{
    public abstract class ReportsControllerBase: AbpController
    {
        protected ReportsControllerBase()
        {
            LocalizationSourceName = PortalConsts.LocalizationSourceName;
        }
    }
}