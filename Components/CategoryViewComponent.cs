using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TT_ECommerce.Data;

namespace TT_ECommerce.Components
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly TT_ECommerceDbContext _context;

        public CategoryViewComponent(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        // Phương thức chính của View Component
        public IViewComponentResult Invoke()
        {
            // Lấy danh sách danh mục sản phẩm
            var categories = _context.TbProductCategories.ToList();

            return View(categories); // Trả về danh sách danh mục
        }
    }
}
