namespace TT_ECommerce.Models.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public IEnumerable<TT_ECommerce.Models.EF.TbOrderDetail> OrderDetails { get; set; }
    }

}
