using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;
namespace TT_ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class StatisticalController : Controller
    {

        private readonly TT_ECommerceDbContext _context;

        public StatisticalController(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetStatistical(string fromDate, string toDate)
        {
            var query = from o in _context.TbOrders
                        join od in _context.TbOrderDetails
                        on o.Id equals od.OrderId
                        join p in _context.TbProducts
                        on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = await query.GroupBy(x => x.CreatedDate.Date) // Sử dụng x.CreatedDate.Date để chỉ lấy ngày
                .Select(x => new
                {
                    Date = x.Key,
                    TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                    TotalSell = x.Sum(y => y.Quantity * y.Price),
                })
                .Select(x => new
                {
                    Date = x.Date,
                    DoanhThu = x.TotalSell,
                    LoiNhuan = x.TotalSell - x.TotalBuy
                })
                .ToListAsync();

            return Json(new { Data = result });
        }

    }
}
