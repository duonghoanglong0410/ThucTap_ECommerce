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
    public class ShopController : Controller
    {
        private readonly TT_ECommerceDbContext _context;
        public ShopController(TT_ECommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? categoryId, string? searchKeyword, string? search, decimal? minPrice, decimal? maxPrice, string? sort, int page = 1, int pageSize = 8)
        {
            // Lấy tất cả danh mục sản phẩm để hiển thị trong dropdown
            ViewBag.Categories = await _context.TbProductCategories.ToListAsync();

            // Tạo danh sách tùy chọn để sắp xếp
            var sortOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "default", Text = "Position" },
                new SelectListItem { Value = "relevance", Text = "Relevance" },
                new SelectListItem { Value = "name_asc", Text = "Name, A to Z" },
                new SelectListItem { Value = "name_desc", Text = "Name, Z to A" },
                new SelectListItem { Value = "price_asc", Text = "Price, low to high" },
                new SelectListItem { Value = "price_desc", Text = "Price, high to low" }
            };

            ViewBag.SortOptions = sortOptions;
            ViewBag.Sort = sort; // Lưu giá trị sort để kiểm tra trong view

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

            // Lọc theo giá
            if (minPrice.HasValue || maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p =>
                    (p.IsSale && p.PriceSale >= (minPrice ?? 0) && p.PriceSale <= (maxPrice ?? decimal.MaxValue)) ||
                    (!p.IsSale && p.Price >= (minPrice ?? 0) && p.Price <= (maxPrice ?? decimal.MaxValue))
                );
            }

            // Sắp xếp theo tùy chọn
            switch (sort)
            {
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(p => p.IsSale ? p.PriceSale : p.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.IsSale ? p.PriceSale : p.Price);
                    break;
                case "name_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Title);
                    break;
                case "name_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Title);
                    break;
                default:
                    productsQuery = productsQuery.OrderBy(p => p.Title); // Sắp xếp mặc định
                    break;
            }

            // Phân trang
            var totalItems = await productsQuery.CountAsync();
            var products = await productsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Tạo đối tượng phân trang
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchKeyword = searchKeyword;

            return View(products);
        }
    }
}
