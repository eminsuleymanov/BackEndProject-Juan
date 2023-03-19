using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JUANBackendProject.DataAccessLayer;
using JUANBackendProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JUANBackendProject.Controllers
{
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProductModal(int? id)
        {

            if (id==null)
            {
                return BadRequest();
            }
            Product product = await _context.Products
                .Include(p=>p.ProductImages.Where(pi=>pi.IsDeleted ==false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
            if (product==null)
            {
                return NotFound();
            }

            return PartialView("_ProductModalPartial",product);
        }

        public async Task<IActionResult> Search(string search)
        {
            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false &&
                   (p.Title.ToLower().Contains(search.ToLower()) || p.Brand.Name.ToLower().Contains(search.ToLower()))).ToListAsync();

            return PartialView("_SearchPartial",products);

        }
    }
}

