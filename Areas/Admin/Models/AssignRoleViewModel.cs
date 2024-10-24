namespace TT_ECommerce.Areas.Admin.Models
{
    public class AssignRoleViewModel
    {
        public string UserId { get; set; } // ID của người dùng
        public string UserName { get; set; } // Tên người dùng
        public List<RoleViewModel> Roles { get; set; } // Danh sách các vai trò có sẵn
        public List<string> UserRoles { get; set; } // Danh sách các vai trò hiện tại của người dùng
        public List<string> SelectedRoles { get; set; } // Danh sách các vai trò được chọn
    }
}