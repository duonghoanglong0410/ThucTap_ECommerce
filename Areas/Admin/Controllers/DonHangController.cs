using Microsoft.AspNetCore.Mvc;

namespace TT_ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")] // Correct attribute for areas
    public class DonHangController : Controller
    {
        // xem đơn hàng, thống kê doanh thu
        public IActionResult Index()
        {
            return View();
        }
    }
}
