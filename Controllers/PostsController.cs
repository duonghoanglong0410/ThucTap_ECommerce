using Microsoft.AspNetCore.Mvc;
using TT_ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TT_ECommerce.Data;

namespace TT_ECommerce.Controllers
{
    public class PostsController : Controller
    {
        private readonly TT_ECommerceDbContext _context;

        public PostsController(TT_ECommerceDbContext context)
        {
            _context = context;
        }
        // GET: /Posts/
        public IActionResult Index()
        {
            var items = _context.TbPosts.ToList();
            return View(items);
        }

        // GET: /Posts/Detail/5
        public IActionResult Detail(int id)
        {
            var item = _context.TbPosts.Find(id);
            if (item != null)
            {
                _context.SaveChanges();  // Chỉ cần save nếu có thay đổi
            }

            return View(item);
        }
    }
}
