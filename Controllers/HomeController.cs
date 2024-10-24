using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TT_ECommerce.Models;
using TT_ECommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TT_ECommerce.Models.EF;
using Microsoft.AspNetCore.Diagnostics;

namespace TT_ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TT_ECommerceDbContext _context; // Khai báo DbContext

        // Inject cả ILogger và DbContext vào constructor
        public HomeController(ILogger<HomeController> logger, TT_ECommerceDbContext context)
        {
            _logger = logger;
            _context = context; // Khởi tạo DbContext
            ViewBag.CartItemCount = GetCartItemCount(); // Gọi GetCartItemCount ở đây
        }

        // Hàm tính tổng số lượng sản phẩm trong giỏ hàng
        private int GetCartItemCount()
        {
            // Kiểm tra nếu _context hoặc DbSet là null
            if (_context == null || _context.TbOrderDetails == null)
            {
                return 0; // Trả về 0 nếu _context hoặc TbOrderDetails không tồn tại
            }

            // Tính tổng số lượng sản phẩm trong giỏ hàng
            var cartItemCount = _context.TbOrderDetails.Sum(od => od.Quantity);
            return cartItemCount;
        }

        public IActionResult Index()
        {
            var categories = _context.TbProductCategories.ToList();

            return View(categories);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Subscribe(TbSubscribe model)
        {
            if (ModelState.IsValid)
            {
                // Lưu thông tin đăng ký vào cơ sở dữ liệu
                _context.TbSubscribes.Add(model);
                _context.SaveChanges();

                // Thông báo thành công
                TempData["SuccessMessage"] = "Subscription successful!";
            }

            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // Action để hiển thị trang lỗi
        [Route("Home/Error")]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFound"); // Trả về view 404 nếu là lỗi 404
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            
        }

    }
}