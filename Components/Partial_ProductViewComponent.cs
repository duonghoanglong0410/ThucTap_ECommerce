using Microsoft.AspNetCore.Mvc;
using TT_ECommerce.Data;

namespace TT_ECommerce.Components
{
    public class Partial_ProductViewComponent : ViewComponent
    {
        private readonly TT_ECommerceDbContext _context;

        public Partial_ProductViewComponent(TT_ECommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var sanPham = await _context.TbProducts.FindAsync(id); // Giả sử bạn có một bảng sản phẩm
            return View(sanPham);
        }
    }
}
