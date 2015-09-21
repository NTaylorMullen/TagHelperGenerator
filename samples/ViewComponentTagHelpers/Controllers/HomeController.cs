using Microsoft.AspNet.Mvc;

namespace ViewComponentTagHelpers.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
