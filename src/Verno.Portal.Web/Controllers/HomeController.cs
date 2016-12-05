using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Verno.Portal.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : ReportsControllerBase
    {
        public ActionResult Index()
        {
            return View("~/wwwroot/index.html"); //Layout of the angular application.
        }
    }
}