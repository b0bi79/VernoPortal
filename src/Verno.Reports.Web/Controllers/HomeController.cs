using Microsoft.AspNetCore.Mvc;

namespace Verno.Reports.Web.Controllers
{
    public class HomeController : ReportsControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}