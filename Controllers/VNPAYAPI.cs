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
        private readonly string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly string tmnCode = "2LCEW8W3"; // Merchant code
        private readonly string hashSecret = "BZ1OAJQ2VCEDS5EWIDBWM1JZDQ3JTJB0"; // Secret key for hashing
        private readonly TT_ECommerceDbContext _context;

        public VNPAYAPI(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/VNPayAPI/{amount}&{infor}&{orderinfor}")]
        public async Task<IActionResult> Payment(string amount, string infor, string orderinfor)
        {
            string clientIPAddress = GetClientIpAddress();
            PayLib pay = new PayLib();

            // Prepare payment request data
            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", amount); // Amount in VND (multiply by 100)
            pay.AddRequestData("vnp_BankCode", ""); // Optional, can be left blank for user selection
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", clientIPAddress); // Client's IP address
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", infor);
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", Url.Action("Index", "Home", null, Request.Scheme));
            pay.AddRequestData("vnp_TxnRef", orderinfor); // Order reference

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return Json(new { url = paymentUrl });
        }

        private string GetClientIpAddress()
        {
            // Use the correct way to get the client IP address
            var remoteIp = HttpContext.Connection.RemoteIpAddress;
            return remoteIp != null ? remoteIp.ToString() : "unknown";
        }

        [Route("/VNpayAPI/paymentconfirm")]
        public async Task<IActionResult> PaymentConfirm()
        {
            if (Request.QueryString.HasValue)
            {
                var queryString = Request.QueryString.Value;
                var json = HttpUtility.ParseQueryString(queryString);

                long orderId = Convert.ToInt64(json["vnp_TxnRef"]);
                string orderInfor = json["vnp_OrderInfo"].ToString();
                long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]);
                string vnp_ResponseCode = json["vnp_ResponseCode"].ToString();
                string vnp_SecureHash = json["vnp_SecureHash"].ToString();
                var pos = Request.QueryString.Value.IndexOf("&vnp_SecureHash");

                bool checkSignature = ValidateSignature(Request.QueryString.Value.Substring(1, pos - 1), vnp_SecureHash, hashSecret);

                if (vnp_ResponseCode == "00" && checkSignature)
                {
                    // Payment successful
                    var cartItems = await _context.TbOrderDetails.Where(c => c.OrderId == orderId).ToListAsync();
                    Console.WriteLine($"Number of items in the cart: {cartItems.Count}");

                    if (cartItems != null && cartItems.Count > 0)
                    {
                        try
                        {
                            // Create a new order
                            var newOrder = new TbOrder
                            {
                                Code = orderInfor,
                                CreatedDate = DateTime.Now,
                                Status = 1, // Assuming 1 means completed
                                TotalAmount = cartItems.Sum(item => item.Price * item.Quantity), // Calculate total amount
                                Quantity = cartItems.Sum(item => item.Quantity) // Calculate total quantity
                            };

                            foreach (var item in cartItems)
                            {
                                // Create a new order detail for each item in the cart
                                var orderDetail = new TbOrderDetail
                                {
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                };

                                newOrder.TbOrderDetails.Add(orderDetail);
                            }

                            // Add the new order to the context and save changes
                            await _context.TbOrders.AddAsync(newOrder);
                            await _context.SaveChangesAsync();

                            // Remove items from the cart after saving the order
                            _context.TbOrderDetails.RemoveRange(cartItems);
                            await _context.SaveChangesAsync();

                            Console.WriteLine($"Removed {cartItems.Count} items.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error saving changes: {ex.Message}");
                        }

                        // Store success message in TempData to display on the Home page
                        TempData["SuccessMessage"] = "Payment successful";

                        // Redirect to the Home page
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Response doesn't match the signature or there was an error in payment
                    return Redirect("LINK_INVALID_RESPONSE");
                }
            }
            // Invalid response
            return Redirect("LINK_INVALID_RESPONSE");
        }

        public bool ValidateSignature(string rspraw, string inputHash, string secretKey)
        {
            string myChecksum = PayLib.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
