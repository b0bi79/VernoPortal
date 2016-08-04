using Abp.AspNetCore.Mvc.Views;

namespace Verno.Reports.Web.Views
{
    public abstract class ReportsRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected ReportsRazorPage()
        {
            LocalizationSourceName = ReportsConsts.LocalizationSourceName;
        }
    }
}
