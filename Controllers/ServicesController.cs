using Microsoft.AspNetCore.Mvc;

namespace TT_ECommerce.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
