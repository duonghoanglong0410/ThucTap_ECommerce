using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using TT_ECommerce.Data;
using TT_ECommerce.Utils;
using TT_ECommerce.Models.EF;
using System.Security.Claims;
namespace TT_ECommerce.Controllers
{
    public class VNPAYAPI : Controller
    {
        public string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
       
        public string tmnCode = "2LCEW8W3";
        public string hashSecret = "BZ1OAJQ2VCEDS5EWIDBWM1JZDQ3JTJB0";
        public IActionResult Index()
        {
            return View();
        }

        [Route("/VNPayAPI/{amount}&{infor}&{orderinfor}")]
        public async Task<IActionResult> Payment(string amount, string infor, string orderinfor)
        {
            string hostName = System.Net.Dns.GetHostName();
            string clientIPAddress = System.Net.Dns.GetHostAddresses(hostName).GetValue(0).ToString();
            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", amount); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", "172.24.96.1"); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", infor); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", Url.Action("Index", "Home", null, Request.Scheme)); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", orderinfor); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            // string encodedPaymentUrl = Uri.EscapeUriString(paymentUrl);
            // return Redirect(paymentUrl);
            return Json(new { url = paymentUrl });
        }
        private readonly TT_ECommerceDbContext _context;

        public VNPAYAPI(TT_ECommerceDbContext context)
        {
            _context = context;
        }
        [Route("/VNpayAPI/paymentconfirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                var queryString = Request.QueryString.Value;
                var json = HttpUtility.ParseQueryString(queryString);

                long orderId = Convert.ToInt64(json["vnp_TxnRef"]); // Mã hóa đơn
                string orderInfor = json["vnp_OrderInfo"].ToString(); // Thông tin giao dịch
                long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]); // Mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = json["vnp_ResponseCode"].ToString(); // Mã phản hồi
                string vnp_SecureHash = json["vnp_SecureHash"].ToString(); // Hash của dữ liệu trả về
                var pos = Request.QueryString.Value.IndexOf("&vnp_SecureHash");

                bool checkSignature = ValidateSignature(Request.QueryString.Value.Substring(1, pos - 1), vnp_SecureHash, hashSecret); // Kiểm tra chữ ký

                if (vnp_ResponseCode == "00")
                {
                    // Thanh toán thành công
                    var cartItems = _context.TbOrderDetails.Where(c => c.OrderId == orderId).ToList();
                    Console.WriteLine($"Số lượng mục trong giỏ hàng: {cartItems.Count}");

                    if (cartItems != null && cartItems.Count > 0)
                    {
                        try
                        {
                            _context.TbOrderDetails.RemoveRange(cartItems);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"Đã xóa {cartItems.Count} mục.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi lưu thay đổi: {ex.Message}");
                        }

                        // Kiểm tra số lượng mục còn lại
                        var remainingItems = _context.TbOrderDetails.Where(c => c.OrderId == orderId).ToList();
                        Console.WriteLine($"Số lượng mục còn lại trong giỏ hàng: {remainingItems.Count}");
                    }

                    // Lưu thông điệp vào TempData để hiển thị trên trang Home
                    TempData["SuccessMessage"] = "Thanh toán thành công";

                    // Điều hướng về trang Home
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    // Phản hồi không khớp với chữ ký
                    return Redirect("LINK_PHAN_HOI_KHONG_HOP_LE");
                }
            }
            // Phản hồi không hợp lệ
            return Redirect("LINK_PHAN_HANH_KHONG_HOP_LE");
        }



        public bool ValidateSignature(string rspraw, string inputHash, string secretKey)
        {
            string myChecksum = PayLib.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
        
    }
}
