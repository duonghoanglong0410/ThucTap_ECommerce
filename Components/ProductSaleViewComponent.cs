using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;

namespace TT_ECommerce.Components
{
    public class ProductSaleViewComponent : ViewComponent
    {
        private readonly TT_ECommerceDbContext _context;

        public ProductSaleViewComponent(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        // Phương thức chính của View Component
        public IViewComponentResult Invoke(int topCount = 10)
        {
            // Lấy danh sách 10 sản phẩm có giá rẻ nhất
            var cheapProducts = _context.TbProducts
                .OrderBy(p => p.PriceSale > 0 ? p.PriceSale : p.Price) // Sắp xếp theo giá sale nếu có, nếu không thì theo giá gốc
                .Take(topCount)
                .ToList();

            return View(cheapProducts); // Trả về danh sách sản phẩm có giá rẻ nhất
        }
    }
}
