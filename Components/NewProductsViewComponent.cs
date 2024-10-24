using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;

namespace TT_ECommerce.Components
{
    public class NewProductsViewComponent : ViewComponent
    {
        private readonly TT_ECommerceDbContext _context;

        public NewProductsViewComponent(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(int count = 20) // Số lượng sản phẩm hiển thị
        {
            // Lấy danh sách sản phẩm mới nhất
            var newProducts = _context.TbProducts
                .OrderByDescending(p => p.CreatedDate) // sử dụng trường CreateDate để xác định sản phẩm mới
                .Take(count)
                .ToList();

            return View(newProducts); // Trả về danh sách sản phẩm mới
        }
    }
}
