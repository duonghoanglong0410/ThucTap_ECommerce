using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TT_ECommerce.Models.EF;

namespace TT_ECommerce.Models.ViewModels
{
    public class PostsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Detail { get; set; }

        public string? Image { get; set; }

        public int CategoryId { get; set; }

        public string? SeoTitle { get; set; }

        public string? SeoDescription { get; set; }

        public string? SeoKeywords { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string? Modifiedby { get; set; }

        public string? Alias { get; set; }

        public bool IsActive { get; set; }

        // Thêm danh sách danh mục nếu cần
    }
}
