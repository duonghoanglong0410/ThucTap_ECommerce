using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TT_ECommerce.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
		public IActionResult Error(int statusCode)
		{
			if (statusCode == 404)
			{
				return View("NotFound"); // Chuyển đến view NotFound.cshtml
			}
			return View("Error"); // Trang lỗi chung
		}
	}
}
