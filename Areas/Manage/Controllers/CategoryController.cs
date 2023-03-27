using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JUANBackendProject.DataAccessLayer;
using JUANBackendProject.Models;
using JUANBackendProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JUANBackendProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class CategoryController : Controller
    {

        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Category> categories = _context.Categories
                .Include(p => p.Products.Where(p => p.IsDeleted == false))
                .Where(p => p.IsDeleted == false).OrderByDescending(b => b.Id);
            return View(PageNatedList<Category>.Create(categories, pageIndex, 3));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id,int pageIndex=1)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.Include(b => b.Products.Where(p => p.IsDeleted == false)).FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);
            if (category == null) return NotFound();
            return View(category);

        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (await _context.Categories.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower().Contains(category.Name.Trim().ToLower())))
            {
                ModelState.AddModelError("Name", $" {category.Name} category artiq movcuddur.");

                return View(category);
            }
            category.Name = category.Name.Trim();
            category.CreatedBy = "System";
            category.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);
            if (category == null) return NotFound();
            return View(category);

        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (id == null) return BadRequest();
            if (id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (category == null) return NotFound();
            if (await _context.Categories.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower().Contains(category.Name.Trim().ToLower()) && category.Id != b.Id))
            {
                ModelState.AddModelError("Name", $" {category.Name} category artiq movcudur.");

                return View(category);
            }
            dbCategory.Name = category.Name.Trim();
            dbCategory.UpdatedBy = "System";
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.Include(b => b.Products.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (category == null) return NotFound();


            return View(category);
        }

        [HttpGet]

        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.Include(b => b.Products.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (category == null) return NotFound();

            category.IsDeleted = true;
            category.DeletedBy = "System";
            category.DeletedAt = DateTime.UtcNow.AddHours(4);

            foreach (Product product in category.Products)
            {
                product.IsDeleted = true;
                product.DeletedBy = "System";
                product.DeletedAt = DateTime.UtcNow.AddHours(4);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}


