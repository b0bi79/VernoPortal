using Abp.Application.Services;

namespace Verno.Reports
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ReportsAppServiceBase : ApplicationService
    {
        protected ReportsAppServiceBase()
        {
            LocalizationSourceName = ReportsConsts.LocalizationSourceName;
        }
    }
}