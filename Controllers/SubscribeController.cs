using Microsoft.AspNetCore.Mvc;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;
using System.Threading.Tasks;

namespace TT_ECommerce.Controllers
{
    public class Subscribe : Controller
    {
        private readonly TT_ECommerceDbContext _context;

        public Subscribe(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: Subscribe
        public IActionResult Index()
        {
            return View();
        }

        // POST: Subscribe/Create
        [HttpPost]
        public async Task<IActionResult> Create(TbSubscribe subscriber)
        {
            if (ModelState.IsValid)
            {
                await _context.TbSubscribes.AddAsync(subscriber);
                await _context.SaveChangesAsync();
                ViewBag.Message = "Đăng ký thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Đăng ký thất bại!";
            return View(subscriber);
        }
    }
}
