using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TT_ECommerce.Data;

namespace TT_ECommerce.Components
{
    public class CartViewComponent : ViewComponent
    {
        private readonly TT_ECommerceDbContext _context; // Thay thế ApplicationDbContext bằng context của bạn

        public CartViewComponent(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy giỏ hàng từ cơ sở dữ liệu
            var cartItems = await _context.TbOrders
                .Include(o => o.TbOrderDetails) // Thay thế nếu bạn có quan hệ khác
                    .ThenInclude(d => d.Product) // Giả sử TbOrderDetail có một mối quan hệ với TbProduct
                .ToListAsync();

            // Tính tổng số lượng sản phẩm

            // Truyền số lượng sản phẩm vào ViewBag
            ViewBag.CartItemCount = GetCartItemCount();
            return View();
        }
        private async Task<int> GetCartItemCountAsync()
        {
            return await _context.TbOrderDetails
                .Where(od => od.Order.Status == null) // Giả sử chúng ta chỉ muốn đếm các đơn hàng hoạt động
                .SumAsync(od => od.Quantity);
        }
        public int GetCartItemCount()
        {
            // Tính tổng số lượng sản phẩm trong tất cả các chi tiết đơn hàng
            var cartItemCount = _context.TbOrderDetails.Sum(od => od.Quantity);
            return cartItemCount;
        }
    }
}
