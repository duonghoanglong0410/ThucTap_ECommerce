using System;
using System.Collections.Generic;

namespace TT_ECommerce.Models.EF;

public partial class TbProduct
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? ProductCode { get; set; }

    public string? Description { get; set; }

    public string? Detail { get; set; }

    public string? Image { get; set; }

    public decimal Price { get; set; }

    public decimal? PriceSale { get; set; }

    public int Quantity { get; set; }

    public bool IsHome { get; set; }

    public bool IsSale { get; set; }

    public bool IsFeature { get; set; }

    public bool IsHot { get; set; }

    public int ProductCategoryId { get; set; }

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    public string? SeoKeywords { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string? Modifiedby { get; set; }

    public string? Alias { get; set; }

    public bool IsActive { get; set; }

    public int ViewCount { get; set; }

    public decimal OriginalPrice { get; set; }

    public virtual TbProductCategory ProductCategory { get; set; } = null!;

    public virtual ICollection<TbOrderDetail> TbOrderDetails { get; set; } = new HashSet<TbOrderDetail>();

    public virtual ICollection<TbProductImage> TbProductImages { get; set; } = new List<TbProductImage>();
}
