using System;
using System.ComponentModel.DataAnnotations;
using TT_ECommerce.Models.EF;

namespace TT_ECommerce.Models.ViewModels
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public string? SeoTitle { get; set; }

        public string? SeoDescription { get; set; }

        public string? SeoKeywords { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string? Modifiedby { get; set; }

        public string Alias { get; set; } = null!;

        public virtual ICollection<TbProduct> TbProducts { get; set; } = new List<TbProduct>();
    }

}
