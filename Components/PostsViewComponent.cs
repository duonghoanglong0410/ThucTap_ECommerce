using Microsoft.AspNetCore.Mvc;
using TT_ECommerce.Data;
using TT_ECommerce.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TT_ECommerce.Components
{
    public class PostsViewComponent : ViewComponent // Kế thừa từ ViewComponent thay vì Controller
    {
        private readonly TT_ECommerceDbContext _context;

        public PostsViewComponent(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        // Phương thức InvokeAsync để lấy dữ liệu và trả về view
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _context.TbPosts.ToListAsync(); // Lấy danh sách bài viết từ database
            return View("Default", posts); // Trả về view "Default" với danh sách bài viết
        }
    }
}
