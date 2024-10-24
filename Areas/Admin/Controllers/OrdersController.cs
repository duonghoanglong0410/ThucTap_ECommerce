using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;
using TT_ECommerce.Models.ViewModels;
using TT_ECommerce.Models;
using X.PagedList;

namespace TT_ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly TT_ECommerceDbContext _context;

        public OrdersController(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(int? page)
        {
            var items = _context.TbOrders.OrderByDescending(x => x.CreatedDate).ToList();

            if (page == null)
            {
                page = 1;
            }
            var pageNumber = page ?? 1;
            var pageSize = 10;
            ViewBag.PageSize = pageSize;
            ViewBag.Page = pageNumber;
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> View(int id)
        {
            var order = await _context.TbOrders
                .Include(o => o.TbOrderDetails) // Bao gồm chi tiết đơn hàng
                .ThenInclude(od => od.Product) // Nạp kèm thông tin sản phẩm cho từng chi tiết đơn hàng
                .FirstOrDefaultAsync(o => o.Id == id);

            return View(order); // Truyền model TbOrder vào view chính
        }
        public async Task<IActionResult> Partial_Product(int id)
        {
            var items = await _context.TbOrderDetails
                .Include(od => od.Product) // Nạp kèm thông tin sản phẩm
                .Where(x => x.OrderId == id)
                .ToListAsync();

            return PartialView(items);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTT(int id, int trangthai)
        {
            var item = await _context.TbOrders.FindAsync(id);
            if (item != null)
            {
                item.TypePayment = trangthai;

                _context.TbOrders.Update(item); // Cập nhật thông tin đối tượng

                await _context.SaveChangesAsync(); // Lưu thay đổi một cách bất đồng bộ

                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }
        public async Task<IEnumerable<RevenueStatisticViewModel>> ThongKe(string fromDate, string toDate)
        {
            var query = from o in _context.TbOrders
                        join od in _context.TbOrderDetails on o.Id equals od.OrderId
                        join p in _context.TbProducts on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.Price
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime start = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate >= start);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = await query
                .GroupBy(x => x.CreatedDate.Date)
                .Select(r => new
                {
                    Date = r.Key,
                    TotalBuy = r.Sum(x => x.OriginalPrice * x.Quantity),
                    TotalSell = r.Sum(x => x.Price * x.Quantity)
                })
                .Select(x => new RevenueStatisticViewModel
                {
                    Date = x.Date,
                    Benefit = x.TotalSell - x.TotalBuy,
                    Revenues = x.TotalSell
                })
                .ToListAsync(); // Chuyển đổi thành danh sách bất đồng bộ

            return result;
        }


        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbOrder = await _context.TbOrders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tbOrder == null)
            {
                return NotFound();
            }

            return View(tbOrder);
        }


        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbOrder = await _context.TbOrders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tbOrder == null)
            {
                return NotFound();
            }

            return View(tbOrder);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbOrder = await _context.TbOrders.FindAsync(id);
            if (tbOrder != null)
            {
                _context.TbOrders.Remove(tbOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbOrderExists(int id)
        {
            return _context.TbOrders.Any(e => e.Id == id);
        }
    }
}
