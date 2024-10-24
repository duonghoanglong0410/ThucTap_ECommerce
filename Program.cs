using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;
using TT_ECommerce.Services;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Authentication với Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/LoginAdmin";  // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/LoginAdmin/Logout";  // Đường dẫn đến trang đăng xuất
    });

// Cấu hình kết nối đến cơ sở dữ liệu SQL Server
builder.Services.AddDbContext<TT_ECommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm các dịch vụ cho EmailService và OtpService
builder.Services.AddTransient<EmailService>();
builder.Services.AddTransient<OtpService>();

// Cấu hình Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<TT_ECommerceDbContext>()
    .AddDefaultTokenProviders();

// Thêm dịch vụ MVC và HttpContextAccessor
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Cấu hình pipeline HTTP
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Kích hoạt Session middleware
app.UseAuthentication(); // Kích hoạt Authentication middleware
app.UseAuthorization(); // Kích hoạt Authorization middleware

// Middleware xử lý lỗi 404
app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode=404");

// Cấu hình các route trong ứng dụng
app.UseEndpoints(endpoints =>
{
    // Route cho các Area
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    // Route cho Shop
    endpoints.MapControllerRoute(
        name: "shop",
        pattern: "Shop",
        defaults: new { controller = "TbProducts", action = "Index" });

    // Route mặc định
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

// Chạy ứng dụng
app.Run();
