using System;
using System.Collections.Generic;

namespace TT_ECommerce.Models.EF;

public partial class TbProductCategory
{
    public int Id { get; set; }

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
