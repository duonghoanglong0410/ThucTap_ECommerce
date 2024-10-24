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
         

            // Tìm sản phẩm trong cơ sở dữ liệu
            var product = _context.TbProducts.Find(productId);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("User is not logged in"); // Hoặc có thể chuyển hướng đến trang đăng nhập
            }
          
                // Lấy thông tin người dùng từ Claims
                var customerName = User.FindFirst(ClaimTypes.Name)?.Value; // Tên khách hàng
                var phone = "Null"; // Số điện thoại, nếu có lưu trong Claims
                var address = "Null"; // Địa chỉ, nếu có lưu trong Claims
                var email = User.FindFirst(ClaimTypes.Email)?.Value; 
          
           
            // Kiểm tra thông tin khách hàng
            if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(email) /*|| string.IsNullOrEmpty(phone)*/ /*|| string.IsNullOrEmpty(address)*/)
            {
                return BadRequest("Customer Name or Email are required");
            }

            //// Tạo mã đơn hàng (có thể tuỳ chỉnh mã theo ý muốn)
            var orderCode = "ORD-" + DateTime.Now.Ticks;

            // Tạo đơn hàng mới
            var order = new TbOrder
            {
                Code = orderCode, // Mã đơn hàng
                CustomerName = customerName,
                Phone = phone,
                Address = address,
                TotalAmount = product.Price * quantity, // Tính tổng tiền dựa trên giá sản phẩm
                Quantity = quantity,
                Email = email,
                TypePayment = typePayment,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Status = 0 // Đặt trạng thái mặc định, ví dụ 0 = 'Pending'
            };

            // Thêm đơn hàng vào cơ sở dữ liệu
            _context.TbOrders.Add(order);
            _context.SaveChanges(); // Lưu đơn hàng trước để có ID

            // Tạo chi tiết đơn hàng
            var orderDetail = new TbOrderDetail
            {
                OrderId = order.Id, // ID của đơn hàng vừa tạo
                ProductId = product.Id, // ID sản phẩm
                Price = product.Price, // Giá sản phẩm
                Quantity = quantity // Số lượng sản phẩm
            };

            // Thêm chi tiết đơn hàng vào cơ sở dữ liệu
            _context.TbOrderDetails.Add(orderDetail);
            _context.SaveChanges(); // Lưu chi tiết đơn hàng vào database

            TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng thành công!"; // Thêm thông báo thành công
            return RedirectToAction("Index", new { id = productId }); // Chuyển hướng đến trang chi tiết sản phẩm
        }



        public IActionResult Index(int id)
            {
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
