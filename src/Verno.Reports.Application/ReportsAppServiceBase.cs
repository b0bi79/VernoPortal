using Abp.Application.Services;

namespace Verno.Reports
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class IdentityAppServiceBase : ApplicationService
    {
        protected IdentityAppServiceBase()
        {
            LocalizationSourceName = ReportsConsts.LocalizationSourceName;
        }
    }
}