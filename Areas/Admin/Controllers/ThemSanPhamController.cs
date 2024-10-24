using Microsoft.AspNetCore.Mvc;
using TT_ECommerce.Data;

namespace TT_ECommerce.Areas.Admin.Controllers
{
	public class ThemSanPhamController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
