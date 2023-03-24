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

        public async Task<IActionResult> Index(int? categoryId, int pageIndex = 1, double minPrice = 0, double maxPrice = 0)
        {
            IQueryable<Product> products = _context.Products
                .Where(p => p.IsDeleted == false);
            if (minPrice > 0 || maxPrice > 0)
            {
                products = products.Where(p => p.DiscountedPrice > 0 ?
                    p.DiscountedPrice >= minPrice && p.DiscountedPrice <= (minPrice == 0 ? 400 : maxPrice) :
                    p.Price >= minPrice && p.Price <= (maxPrice == 0 ? 400 : maxPrice));
            }

            ShopVM shopVM = new()
            {
                CategoryId = categoryId,
                Categories = await _context.Categories
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .Where(c => c.IsDeleted == false).ToListAsync(),

                Products = PageNatedList<Product>.Create(products.Where(p => (categoryId == null || p.CategoryId == categoryId) && p.IsDeleted == false), pageIndex, 6)

            };
            return View(shopVM);
        }

        public async Task<IActionResult> Filter(string? range = "", int pageIndex = 1)
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
            //IEnumerable<Category> categories = await _context.Categories
            //    .Include(c => c.Products.Where(p => p.IsDeleted == false))
            //    .Where(c => c.IsDeleted == false).ToListAsync();

            //IEnumerable<Product> products =await _context.Products
            //    .Where(p => p.IsDeleted == false).ToListAsync();

            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false && (p.DiscountedPrice > 0 ?
                    p.DiscountedPrice >= minValue && p.DiscountedPrice <= (minValue == 0 ? 400 : maxValue) :
                    p.Price >= minValue && p.Price <= (maxValue == 0 ? 400 : maxValue))).ToListAsync();

            //return RedirectToAction("Index", new { pageIndex = pageIndex, minPrice = minValue, maxPrice = maxValue });
            return PartialView("_ShopListPartial", products);

        }

        //    public async Task<IActionResult> Index(int? categoryId, int pageIndex = 1)
        //    {
        //        // Code to retrieve all products and categories
        //        // ...

        //        var products = _context.Products.Where(p => p.IsDeleted == false);

        //        var shopVM = new ShopVM
        //        {
        //            CategoryId = categoryId,
        //            Categories = await _context.Categories
        //                .Include(c => c.Products.Where(p => p.IsDeleted == false))
        //                .Where(c => c.IsDeleted == false)
        //                .ToListAsync(),
        //            Products = await GetFilteredProductsAsync(products, categoryId, pageIndex)
        //        };

        //        return View(shopVM);
        //    }

        //    private async Task<PageNatedList<Product>> GetFilteredProductsAsync(IQueryable<Product> products, int? categoryId, int pageIndex, string range = "")
        //    {
        //        // Code to filter products based on range and category
        //        // ...

        //        var filteredProducts = products.Where(/* your filter conditions */);

        //        return PageNatedList

        //}
    }
}
