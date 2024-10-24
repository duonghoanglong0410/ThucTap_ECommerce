using System;
using System.Collections.Generic;

namespace TT_ECommerce.Models.EF;

public partial class ThongKe
{
    public int Id { get; set; }

    public DateTime ThoiGian { get; set; }

    public long SoTruyCap { get; set; }
}
