using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;

namespace TT_ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductManagerController : Controller
    {
        private readonly TT_ECommerceDbContext _context;

        public ProductManagerController(TT_ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            var productsQuery = _context.TbProducts.Include(t => t.ProductCategory);

            var totalItems = await productsQuery.CountAsync();

            var products = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
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

        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.ProductCategories = new SelectList(_context.TbProductCategories.ToList(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateProduct(TbProduct product, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgProducts");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                if (Image != null && Image.Length > 0)
                {
                    string fileName = Path.GetFileName(Image.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(fileStream);
                    }
                    product.Image = "/imgProducts/" + fileName;
                }
                product.ProductCode = "sanpham" + product.Id;
                product.CreatedBy = User.Identity.Name;
                product.Modifiedby = User.Identity.Name;
                product.CreatedDate = DateTime.Now;
                product.ModifiedDate = DateTime.Now;

                _context.TbProducts.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductCategories = new SelectList(_context.TbProductCategories.ToList(), "Id", "Title");
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.TbProducts.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.ProductCategories = new SelectList(_context.TbProductCategories.ToList(), "Id", "Title", product.ProductCategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(TbProduct product, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.TbProducts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (Image != null && Image.Length > 0)
                {
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgProducts");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string fileName = Path.GetFileName(Image.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(fileStream);
                    }

                    product.Image = "/imgProducts/" + fileName;
                }
                else
                {
                    product.Image = existingProduct.Image;
                }

                product.CreatedDate = existingProduct.CreatedDate; // Giữ lại ngày tạo cũ
                product.ModifiedDate = DateTime.Now;

                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.ProductCategories = new SelectList(_context.TbProductCategories.ToList(), "Id", "Title", product.ProductCategoryId);
            return View(product);
        }
    }
}
