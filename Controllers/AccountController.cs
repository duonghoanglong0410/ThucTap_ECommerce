using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TT_ECommerce.Models;
using TT_ECommerce.Services;

namespace TT_ECommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly OtpService _otpService;
        private readonly EmailService _emailService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, OtpService otpService, EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _otpService = otpService;
            _emailService = emailService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user;
                string userEmail = string.Empty;

                if (new EmailAddressAttribute().IsValid(model.UsernameOrEmail))
                {
                    user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
                    userEmail = model.UsernameOrEmail;
                }
                else
                {
                    user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
                    if (user != null)
                    {
                        userEmail = user.Email;
                    }
                }

                if (user != null)
                {
                    // Kiểm tra xem cookie đã lưu trạng thái OTP xác thực chưa
                    var otpVerifiedCookie = Request.Cookies[$"OtpVerified_{user.Id}"];
                    if (otpVerifiedCookie != null)
                    {
                        // Nếu OTP đã được xác thực trước đó, đăng nhập người dùng mà không yêu cầu OTP
                        await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }

                    // Nếu chưa xác thực OTP trên thiết bị này, kiểm tra mật khẩu và yêu cầu OTP
                    var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (passwordCheck)
                    {
                        var otp = _otpService.GenerateOtp();
                        try
                        {
                            await _emailService.SendEmailAsync(userEmail, "OTP Verification", $"Your OTP is: {otp}");

                            HttpContext.Session.SetString("OtpEmail", userEmail);
                            HttpContext.Session.SetString("Otp", otp);

                            return RedirectToAction("VerifyOtp", new { email = userEmail });
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, "Error sending OTP email. Please try again later.");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyOtp(string email)
        {
            // Hiển thị trang xác thực OTP
            return View(new VerifyOtpViewModel { Email = email });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var storedOtp = HttpContext.Session.GetString("Otp");
                var email = HttpContext.Session.GetString("OtpEmail");

                if (storedOtp != null && email != null && storedOtp == model.Otp)
                {
                    // Xóa OTP khỏi session sau khi xác thực thành công
                    HttpContext.Session.Remove("Otp");
                    HttpContext.Session.Remove("OtpEmail");

                    // Đăng nhập người dùng
                    var user = await _userManager.FindByEmailAsync(email);
                    await _signInManager.SignInAsync(user, isPersistent: true);

                    // Lưu cookie trạng thái OTP đã được xác thực
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(7),  // Cookie sẽ hết hạn sau 7 NGÀY
                        IsEssential = true,
                        HttpOnly = true,
                        Secure = true // Cookie chỉ được gửi qua HTTPS
                    };

                    Response.Cookies.Append($"OtpVerified_{user.Id}", "true", cookieOptions);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid OTP.");
                }
            }

            return View(model);
        }

        // GET: /Account/Register

        [HttpGet]
        public IActionResult RegistrationConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.ConfirmPassword), "Mật khẩu không trùng khớp.");
                    ModelState.AddModelError(nameof(RegisterViewModel.Password), "Mật khẩu không trùng khớp.");
                }
                else
                {
                    var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

                        try
                        {
                            await _emailService.SendEmailAsync(model.Email, "Confirm your email",
                                $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

                            await _userManager.AddToRoleAsync(user, "USER");
                            await _signInManager.SignInAsync(user, isPersistent: false);

                            return RedirectToAction("RegistrationConfirmation");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending email: {ex.Message}");
                            ModelState.AddModelError(string.Empty, "Có lỗi khi gửi email xác thực. Vui lòng thử lại sau.");
                            return View(model);
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError(string.Empty, "Tên người dùng đã được sử dụng.");
                        }
                        else if (error.Code == "DuplicateEmail")
                        {
                            ModelState.AddModelError(string.Empty, "Email đã được sử dụng.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return View(model);
        }




        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail");
            }

            return View("Error");
        }
        // GET: Forgot Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Forgot Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tìm người dùng dựa trên email
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Nếu người dùng không tồn tại
                if (user == null)
                {
                    // Thông báo lỗi email không tồn tại
                    ModelState.AddModelError(string.Empty, "Email không tồn tại trong hệ thống.");
                    return View(model);
                }
                // Nếu email của người dùng chưa được xác nhận
                else if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Thông báo yêu cầu xác nhận email
                    ModelState.AddModelError(string.Empty, "Email chưa được xác nhận. Vui lòng kiểm tra hộp thư để xác nhận email.");
                    return View(model);
                }

                // Tạo token và link reset mật khẩu
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);

                try
                {
                    // Gửi email reset mật khẩu
                    await _emailService.SendEmailAsync(model.Email, "Reset Password", $"Click here to reset your password: <a href='{resetLink}'>link</a>");
                    
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu không gửi được email
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Có lỗi khi gửi email reset mật khẩu. Vui lòng thử lại sau.");
                    return View(model);
                }
                return Redirect("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        // GET: Forgot Password Confirmation
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: Reset Password
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        // POST: Reset Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        // GET: Reset Password Confirmation
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Get the logged-in user's username or email from the Identity
            var usernameOrEmail = User.Identity?.Name;
            if (usernameOrEmail == null)
            {
                return RedirectToAction("Login");
            }

            IdentityUser user;

            // Kiểm tra xem usernameOrEmail có phải là email hợp lệ không
            if (new EmailAddressAttribute().IsValid(usernameOrEmail))
            {
                // Nếu là email, tìm người dùng theo email
                user = await _userManager.FindByEmailAsync(usernameOrEmail);
            }
            else
            {
                // Nếu là username, tìm người dùng theo username
                user = await _userManager.FindByNameAsync(usernameOrEmail);
            }

            if (user == null)
            {
                // Nếu không tìm thấy người dùng, chuyển hướng về trang đăng nhập
                return RedirectToAction("Login");
            }

            // Chuẩn bị dữ liệu cho ViewModel UserProfile
            var model = new UserProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                // Add other properties as needed (e.g., FirstName, LastName, etc.)
            };

            return View(model);
        }

        // GET: /Account/Logout
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Xóa cookie OTP đã xác thực
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Response.Cookies.Delete($"OtpVerified_{userId}");
            }

            return RedirectToAction("Index", "Home");
        }




    }
}
