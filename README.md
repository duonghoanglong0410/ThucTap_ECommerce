# Dự án ECommerce 
## Giới thiệu
Dự án ECommerce là một ứng dụng web được phát triển bằng ASP.NET Core, tập trung vào việc xây dựng một nền tảng thương mại điện tử hoàn chỉnh với đầy đủ các chức năng cho cả người dùng cuối và quản trị viên.

## Mục tiêu dự án
- Xây dựng một nền tảng thương mại điện tử đáp ứng nhu cầu mua sắm trực tuyến
- Cung cấp giao diện quản trị đầy đủ chức năng để quản lý sản phẩm, đơn hàng và người dùng
- Tạo trải nghiệm người dùng thân thiện và dễ sử dụng
## Cấu trúc dự án
### Giao diện người dùng (Frontend)
- Trang chủ : Hiển thị sản phẩm nổi bật, danh mục sản phẩm
- Trang chi tiết sản phẩm : Hiển thị thông tin chi tiết, hình ảnh, giá cả và đánh giá
- Giỏ hàng : Quản lý sản phẩm đã chọn, tính toán tổng tiền
- Thanh toán : Quy trình thanh toán đơn giản và bảo mật
- Tài khoản người dùng : Đăng ký, đăng nhập, quản lý thông tin cá nhân
### Giao diện quản trị (Admin)
- Bảng điều khiển (Dashboard) : Hiển thị tổng quan về doanh số, đơn hàng, người dùng
- Quản lý sản phẩm : Thêm, sửa, xóa sản phẩm và danh mục
- Quản lý đơn hàng : Theo dõi và xử lý đơn hàng
- Quản lý người dùng : Quản lý tài khoản khách hàng và phân quyền
- Báo cáo và thống kê : Biểu đồ phân tích doanh số, xu hướng mua sắm
- Quản lý nội dung : Quản lý các bài đăng 
## Công nghệ sử dụng
### Frontend
- HTML5/CSS3/JavaScript : Nền tảng cơ bản cho giao diện người dùng
- Bootstrap 5.0.2 : Framework CSS cho thiết kế responsive
- jQuery 3.6.0 : Thư viện JavaScript đơn giản hóa thao tác DOM
- OwlCarousel 2.3.4 : Thư viện slider/carousel
- Font Awesome 5.15.3 : Bộ icon vector
- SweetAlert2 11.0.0 : Thư viện hiển thị thông báo
- jQuery Validation 1.19.3 : Kiểm tra dữ liệu form phía client
- Select2 4.1.0 : Nâng cao trải nghiệm dropdown select
### Backend
- ASP.NET Core 6.0 : Framework phát triển web
- Entity Framework Core 6.0 : ORM (Object-Relational Mapping)
- SQL Server 2019 : Hệ quản trị cơ sở dữ liệu
- Identity Framework : Quản lý xác thực và phân quyền
### Công cụ phát triển
- Visual Studio 2022 : IDE chính
- Git : Hệ thống quản lý phiên bản
- npm/NuGet : Quản lý gói
## Yêu cầu hệ thống
- .NET 6.0 SDK
- SQL Server 2019 hoặc cao hơn
- Visual Studio 2022 (khuyến nghị)
## Hướng dẫn cài đặt
1. Clone repository
   
   ```bash
   git clone https://github.com/username/ThucTap.git
   cd ThucTap
    ```
   ```
2. Khôi phục các gói NuGet
   
   ```bash
   dotnet restore
    ```
3. Cấu hình cơ sở dữ liệu
   
   - Mở file appsettings.json
   - Cập nhật chuỗi kết nối SQL Server
4. Tạo cơ sở dữ liệu
   
   ```bash
   dotnet ef database update
    ```
5. Chạy ứng dụng
   
   ```bash
   dotnet run
    ```
## Hướng dẫn sử dụng
- Trang người dùng : Truy cập vào trang chủ để xem và mua sắm sản phẩm
- Trang quản trị : Đăng nhập vào trang quản trị bằng đường dẫn /admin
## Quy trình phát triển
- Phân tích yêu cầu : Xác định rõ mục tiêu và chức năng
- Thiết kế : Xây dựng cấu trúc cơ sở dữ liệu và giao diện
- Phát triển : Lập trình các chức năng
- Kiểm thử : Đảm bảo chất lượng và hiệu suất
## Lưu ý phát triển
- Tuân thủ quy tắc đặt tên và cấu trúc dự án
- Sử dụng Entity Framework để tương tác với cơ sở dữ liệu
- Tách biệt logic nghiệp vụ và giao diện người dùng
- Kiểm tra bảo mật trước khi triển khai