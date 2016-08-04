using Abp.AspNetCore.Mvc.Controllers;

namespace Verno.Reports.Web.Controllers
{
    public abstract class ReportsControllerBase: AbpController
    {
        protected ReportsControllerBase()
        {
            LocalizationSourceName = ReportsConsts.LocalizationSourceName;
        }
    }
}