using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;

namespace TT_ECommerce.Controllers
{
    public class TbProductsController : Controller
    {
        private readonly TT_ECommerceDbContext _context;

        public TbProductsController(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        // GET: TbProducts
        public async Task<IActionResult> Index(string? categoryId, string? searchKeyword, string? search, decimal? minPrice, decimal? maxPrice, string? sortOrder, int page = 1, int pageSize = 6)
        {
            // Lấy tất cả danh mục sản phẩm để hiển thị trong dropdown
            ViewBag.Categories = await _context.TbProductCategories.ToListAsync();

            // Truy vấn sản phẩm từ database
            var productsQuery = _context.TbProducts
                .Include(p => p.ProductCategory)
                .Include(p => p.TbProductImages)
                .AsQueryable();

            // Lọc theo danh mục
            if (!string.IsNullOrEmpty(categoryId))
            {
                var selectedCategoryIds = categoryId.Split(',').Select(int.Parse).ToList();
                productsQuery = productsQuery.Where(p => selectedCategoryIds.Contains(p.ProductCategoryId));
            }

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
            }
            // Lọc theo từ khóa tìm kiếm sản phẩm
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                productsQuery = productsQuery.Where(p => p.Title.Contains(searchKeyword) || p.Description.Contains(searchKeyword));
            }
            // Lọc theo giá
            if (minPrice.HasValue || maxPrice.HasValue)
            {
                // Nếu sản phẩm có giá khuyến mãi, lọc theo PriceSale, nếu không thì lọc theo Price
                productsQuery = productsQuery.Where(p =>
                    (p.IsSale && p.PriceSale >= (minPrice ?? 0) && p.PriceSale <= (maxPrice ?? decimal.MaxValue)) ||
                    (!p.IsSale && p.Price >= (minPrice ?? 0) && p.Price <= (maxPrice ?? decimal.MaxValue))
                );
            }

            // Sắp xếp theo giá
            switch (sortOrder)
            {
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(p => p.IsSale ? p.PriceSale : p.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.IsSale ? p.PriceSale : p.Price);
                    break;
                default:
                    productsQuery = productsQuery.OrderBy(p => p.Title); // Sắp xếp mặc định theo tên sản phẩm
                    break;
            }

            // Phân trang
            var totalItems = await productsQuery.CountAsync();
            var products = await productsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Tạo đối tượng phân trang
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchKeyword = searchKeyword;

            return View(products);
        }

        // GET: TbProducts/Details/5
        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbProduct = await _context.TbProducts
                .Include(t => t.ProductCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tbProduct == null)
            {
                return NotFound();
            }

            return View(tbProduct);
        }

    }
}
