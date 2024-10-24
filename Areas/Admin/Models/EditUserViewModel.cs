using System.ComponentModel.DataAnnotations;

namespace TT_ECommerce.Areas.Admin.Models
{
    public class EditUserViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string UserId { get; set; }

        public string PhoneNumber { get; set; } // Thêm trường để hiển thị số điện thoại

        public string Password { get; set; } // Thêm trường để cho phép thay đổi mật khẩu
    }

}

