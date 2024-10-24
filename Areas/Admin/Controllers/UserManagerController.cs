using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using TT_ECommerce.Areas.Admin.Models;

namespace TT_ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserManagerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagerController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. Hiển thị danh sách người dùng
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoleViewModels = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoleViewModels.Add(new UserRoleViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList() // Chuyển đổi danh sách các vai trò thành danh sách string
                });
            }

            return View(userRoleViewModels);  // Truyền danh sách UserRoleViewModel vào view
        }
        // 2. Gán vai trò cho người dùng
        [HttpGet]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            Console.WriteLine(user != null ? "User found" : "User not found");
            Console.WriteLine("UserName: " + user.UserName);
            Console.WriteLine("Email: " + user.Email);
            // Lấy vai trò hiện tại của người dùng
            var currentRoles = await _userManager.GetRolesAsync(user);

            var model = new AssignRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = _roleManager.Roles.Select(r => new RoleViewModel { RoleId = r.Id, RoleName = r.Name }).ToList(),
                UserRoles = currentRoles.ToList(),
                SelectedRoles = currentRoles.ToList() // Chọn các vai trò hiện tại
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy vai trò hiện tại của người dùng
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Xóa tất cả vai trò hiện tại của người dùng
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                // Nếu có lỗi khi xóa vai trò, tạo lại model và hiển thị thông báo lỗi
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(new AssignRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = _roleManager.Roles.Select(r => new RoleViewModel { RoleId = r.Id, RoleName = r.Name }).ToList(),
                    UserRoles = currentRoles.ToList(),
                    SelectedRoles = selectedRoles
                });
            }

            // Gán các vai trò mới
            if (selectedRoles != null && selectedRoles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (addResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                // Nếu có lỗi trong quá trình gán vai trò mới, tạo lại model và hiển thị thông báo lỗi
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Nếu không có vai trò nào được chọn hoặc có lỗi, tạo lại model để hiển thị trong view
            var model = new AssignRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = _roleManager.Roles.Select(r => new RoleViewModel { RoleId = r.Id, RoleName = r.Name }).ToList(),
                UserRoles = currentRoles.ToList(),
                SelectedRoles = selectedRoles
            };

            return View(model);
        }
        [Route("EditUser")]
        [HttpGet]
        public async Task<IActionResult> EditUser(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            // Tìm người dùng bằng userId
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Khởi tạo model với thông tin người dùng
            var model = new EditUserViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber // Lấy thông tin số điện thoại từ người dùng
            };

            // Trả về view với model
            return View(model); // Model phải được truyền vào đây
        }


        [Route("EditUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            // Kiểm tra tính hợp lệ của model dựa trên các DataAnnotations trong EditUserViewModel
            if (!ModelState.IsValid)
            {
                return View(model); // Trả lại view với lỗi
            }

            // Tìm người dùng bằng UserId
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Kiểm tra email có bị trùng lặp với người dùng khác không
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null && existingUserByEmail.Id != model.UserId)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            // Kiểm tra tên người dùng có bị trùng lặp với người dùng khác không
            var existingUserByName = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserByName != null && existingUserByName.Id != model.UserId)
            {
                ModelState.AddModelError("UserName", "Username already exists.");
                return View(model);
            }

            // Cập nhật thông tin người dùng
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber; // Cập nhật số điện thoại

            // Cập nhật thông tin người dùng
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Nếu có yêu cầu thay đổi mật khẩu
            if (!string.IsNullOrEmpty(model.Password))
            {
                // Xóa mật khẩu cũ và đặt mật khẩu mới
                var passwordValidator = new PasswordValidator<IdentityUser>();
                var passwordResult = await passwordValidator.ValidateAsync(_userManager, user, model.Password);

                if (passwordResult.Succeeded)
                {
                    var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                    if (removePasswordResult.Succeeded)
                    {
                        var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
                        if (!addPasswordResult.Succeeded)
                        {
                            foreach (var error in addPasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }
                    }
                    else
                    {
                        foreach (var error in removePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            return RedirectToAction("Index"); // Chuyển hướng về trang Index sau khi cập nhật thành công
        }

        [Route("DeleteUser")]
        [HttpGet]
        public async Task<IActionResult> ConfirmDeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(); // Người dùng không tồn tại
            }

            var model = new DeleteUserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return View(model); // Trả về view để xác nhận xóa
        }

        [Route("DeleteUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound(); // Trả về 404 nếu người dùng không tồn tại
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index"); // Xóa thành công, quay lại danh sách người dùng
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Error"); // Trả về trang lỗi nếu xóa thất bại
        }


    }
}
