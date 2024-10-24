using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
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

        // GET: Admin/ProductManager
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

        // GET: Admin/ProductManager/Details/5
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

        // GET: Admin/ProductManager/CreateProduct
        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.ProductCategories = _context.TbProductCategories.ToList();
            return View();
        }

        // POST: Admin/ProductManager/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(TbProduct pro, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgProducts");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Kiểm tra nếu có tệp hình ảnh được tải lên
                if (Image != null && Image.Length > 0)
                {
                    // Tạo tên tệp ngẫu nhiên để tránh trùng lặp
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    // Lưu tệp vào đường dẫn chỉ định
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }

                    // Lưu đường dẫn tương đối vào cơ sở dữ liệu
                    pro.Image = "/imgProducts/" + fileName;
                }

                // Thiết lập thời gian tạo và sửa đổi
                pro.CreatedDate = DateTime.Now;
                pro.ModifiedDate = DateTime.Now;

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.TbProducts.Add(pro);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Nếu có lỗi trong ModelState, truyền lại danh sách danh mục
            ViewBag.ProductCategories = _context.TbProductCategories.ToList();
            return View(pro);
        }

        //  [Route("EditProduct")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int? id)
        {
            ViewBag.ProductCategories = _context.TbProductCategories.ToList();

            if (id == null)
            {
                return NotFound();
            }

            // Lấy sản phẩm theo id
            var product = await _context.TbProducts.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Truy vấn danh mục sản phẩm
            var categories = await _context.TbProductCategories.ToListAsync();
            if (categories == null || !categories.Any())
            {
                ViewBag.ProductCategories = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Không có danh mục", Value = "" }
            });
            }
            else
            {
                // Truyền danh mục vào ViewBag để hiển thị trong dropdown
                ViewBag.ProductCategories = _context.TbProductCategories.ToList();
            }

            return View(product);
        }

        //  [Route("EditProduct")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(TbProduct product, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                // Lấy sản phẩm hiện tại từ database (không tracking)
                var existingProduct = _context.TbProducts.AsNoTracking().FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Kiểm tra nếu có ảnh mới được upload
                if (Image != null && Image.Length > 0)
                {
                    // Đường dẫn để lưu ảnh
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgProducts");

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Lấy tên file và tạo đường dẫn đầy đủ
                    string fileName = Path.GetFileName(Image.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);

                    // Lưu file vào đường dẫn
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(fileStream);
                    }

                    // Cập nhật đường dẫn ảnh vào cơ sở dữ liệu (đường dẫn tương đối)
                    product.Image = "/imgProducts/" + fileName;
                }
                else
                {
                    // Nếu không có ảnh mới, giữ lại ảnh cũ
                    product.Image = existingProduct.Image;
                }
                // Thiết lập thời gian tạo và sửa đổi
                product.CreatedDate = DateTime.Now;
                product.ModifiedDate = DateTime.Now;

                // Cập nhật sản phẩm
                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, trả lại view với dữ liệu sản phẩm và danh mục
            ViewBag.ProductCategories = _context.TbProductCategories.ToList();

            //   var categories = _context.TbProductCategories.ToList();
            //  ViewBag.ProductCategories = new SelectList(categories, "Id", "Title", product.ProductCategoryId);
            return View(product);
        }

        // GET: Admin/Product/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Truy vấn sản phẩm cần xóa
            var product = await _context.TbProducts
                 .Include(p => p.ProductCategory) // Nếu bạn muốn hiển thị thông tin danh mục sản phẩm
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Trả về view xác nhận xóa với thông tin sản phẩm
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var product = await _context.TbProducts.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Nếu có ảnh được lưu trong thư mục, bạn có thể xóa ảnh đi
            if (!string.IsNullOrEmpty(product.Image))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.Image.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath); // Xóa ảnh sản phẩm khỏi thư mục
                }
            }

            _context.TbProducts.Remove(product); // Xóa sản phẩm khỏi cơ sở dữ liệu
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            return RedirectToAction(nameof(Index)); // Quay lại danh sách sản phẩm sau khi xóa
        }

        private bool TbPostExists(int id)
        {
            return _context.TbProducts.Any(e => e.Id == id);
        }
    }
}