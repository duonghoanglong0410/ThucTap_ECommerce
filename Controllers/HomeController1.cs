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
    public class HomeController1 : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TT_ECommerceDbContext _context; // Khai báo DbContext
        public HomeController1(ILogger<HomeController> logger, TT_ECommerceDbContext context)
        {
            _logger = logger;
            _context = context; // Khởi tạo DbContext
        }
        public IActionResult Index()
        {
            // truyền danh mục để hiển thị ở Partial View
            var categories = _context.TbProductCategories.ToList();

            // Lấy 10 sản phẩm bán chạy nhất



            return View(categories);
        }
    }
}
