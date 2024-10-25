using Microsoft.AspNetCore.Mvc;
using TT_ECommerce.Models; // Import namespace chứa model Product
using System.Linq;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;
using System.Net;
using System.Security.Claims;

namespace TT_ECommerce.Controllers
{
    public class ProductDetailsController : Controller
    {
        private readonly TT_ECommerceDbContext _context; // ApplicationDbContext là DbContext của bạn

        public ProductDetailsController(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, int typePayment)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0");
            }

            var product = _context.TbProducts.Find(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }

            // Lấy thông tin khách hàng từ Claims
            var customerName = "null";
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Customer information is required");
            }

            // Khởi tạo giá trị CreatedDate
            var createdDate = DateTime.Now;

            // Tạo mã đơn hàng
            var orderCode = "ORD-" + DateTime.Now.Ticks;

            var order = new TbOrder
            {
                Code = orderCode,
                CustomerName = customerName,
                Email = email,
                Phone = "0123456789", // Tạm thời điền số điện thoại, nên thay bằng giá trị thực tế
                Address = "123 Main St", // Tạm thời điền địa chỉ, nên thay bằng giá trị thực tế
                TotalAmount = product.Price * quantity,
                Quantity = quantity,
                TypePayment = typePayment,
                CreatedDate = createdDate, // Gán giá trị CreatedDate
                ModifiedDate = createdDate, // Gán giá trị ModifiedDate
                Status = 0
            };

            _context.TbOrders.Add(order);
            _context.SaveChanges();

            var orderDetail = new TbOrderDetail
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Price = product.Price,
                Quantity = quantity
            };

            _context.TbOrderDetails.Add(orderDetail);
            _context.SaveChanges();


            return Json(new { success = true, message = "Product added to cart successfully!" });
        }

        public IActionResult Index(int id)
        {

            //Hiển thị lượt xem sản phẩm
            var item = _context.TbProducts.Find(id);
            if (item != null)
            {
                _context.TbProducts.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                _context.Entry(item).Property(x => x.ViewCount).IsModified = true;
                _context.SaveChanges();
            }

            // Lấy sản phẩm theo id từ cơ sở dữ liệu
            var product = _context.TbProducts.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Truyền product vào view
            return View(product);
        }
    }
}
