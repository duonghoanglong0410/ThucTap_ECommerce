using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TT_ECommerce.Areas.Admin.Models;
using TT_ECommerce.Data;
using TT_ECommerce.Models.EF;

namespace TT_ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")] // Correct attribute for areas
    public class DanhMucSanPhamController : Controller
    {
        private readonly TT_ECommerceDbContext _context;

        public DanhMucSanPhamController(TT_ECommerceDbContext context)
        {
            _context = context;
        }
        // Hiển thị danh sách danh mục sản phẩm
        public IActionResult Index()
        {
            var categories = _context.TbProductCategories.ToList();
            return View(categories);
        }
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TbProductCategory category, IFormFile? Icon)
        {
            if (ModelState.IsValid)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "iconCategorys");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                if (Icon != null && Icon.Length > 0)
                {
                    string fileName = Path.GetFileName(Icon.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Icon.CopyTo(fileStream);
                    }
                    category.Icon = "/iconCategorys/" + fileName;
                }
                category.CreatedDate = DateTime.Now;
                category.ModifiedDate = DateTime.Now;

                _context.TbProductCategories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(int id)
        {   
            //ViewBag.RoleName = new SelectList(_context.Roles.ToList(), "CreatedBy", "RoleName");
            //ViewBag.RoleName = new SelectList(_context.Roles.ToList(), "Modifiedby", "RoleName");
            var danhMuc = _context.TbProductCategories.Find(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc);
        }


        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(TbProductCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(category).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.TbProductCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhMuc = await _context.TbProductCategories.FindAsync(id);
            if (danhMuc != null)
            {
                _context.TbProductCategories.Remove(danhMuc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbPostExists(int id)
        {
            return _context.TbPosts.Any(e => e.Id == id);
        }


    }

}
