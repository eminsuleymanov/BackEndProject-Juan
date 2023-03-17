using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JUANBackendProject.DataAccessLayer;
using JUANBackendProject.Models;
using JUANBackendProject.ViewModels;
using JUANBackendProject.ViewModels.ShopViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace JUANBackendProject.Controllers
{
    public class ShopController : Controller
    {

        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageIndex=1)
        {

            ShopVM shopVM = new ()
            {
                Categories = await _context.Categories.Include(c=>c.Products.Where(p=>p.IsDeleted==false)).Where(c => c.IsDeleted == false).ToListAsync(),
                Products = PageNatedList<Product>.Create(_context.Products.Where(p => p.IsDeleted == false), pageIndex, 3)

            };
            return View(shopVM);
        }

        public async Task<IActionResult> List(string? range = "")
        {
            double minValue = 0;
            double maxValue = 0;
            range = range?.Replace("$", "");
            if (!string.IsNullOrWhiteSpace(range))
            {
                string[] arr = range.Split(" - ");
                minValue = double.Parse(arr[0]);
                maxValue = double.Parse(arr[1]);
            }
            IEnumerable<Product> products = await _context.Products
                .Where(p => p.IsDeleted == false && (p.DiscountedPrice > 0 ?
                p.DiscountedPrice >= minValue &&  p.DiscountedPrice <= (minValue == 0 ? 400 : maxValue)
                :p.Price>= minValue && p.Price <= (maxValue == 0 ? 400 : maxValue))).ToListAsync();
            return PartialView("_ShopListPartial",products);

        } 
    }
}

