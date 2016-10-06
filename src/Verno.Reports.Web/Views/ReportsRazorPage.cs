using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace Verno.Reports.Web.Views
{
    public abstract class ReportsRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected ReportsRazorPage()
        {
            LocalizationSourceName = ReportsConsts.LocalizationSourceName;
        }
    }
}
