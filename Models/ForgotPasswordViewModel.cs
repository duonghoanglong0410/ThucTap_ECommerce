using System.ComponentModel.DataAnnotations;

namespace TT_ECommerce.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
