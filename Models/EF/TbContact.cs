using System;
using System.Collections.Generic;

namespace TT_ECommerce.Models.EF;

public partial class TbContact
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? Message { get; set; }

    public bool IsRead { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public string? Modifiedby { get; set; }
}
