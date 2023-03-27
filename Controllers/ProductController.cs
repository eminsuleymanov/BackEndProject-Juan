using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JUANBackendProject.DataAccessLayer;
using JUANBackendProject.Models;
using JUANBackendProject.ViewModels.BasketViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p=>p.ProductImages.Where(pi=>pi.IsDeleted==false))
                .Include(p=>p.Brand)
                .Include(p=>p.Category)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
            if (product == null) return NotFound();

            return View(product);

        }



        public IActionResult ChangeBasketProductCount(int? id, int count)
        {
            if (id == null)
            {
                return BadRequest();
            }
            if (!_context.Products.Any(p => p.Id == id))
            {
                return NotFound();
            }
            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;
            if (basket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                basketVMs.Find(p => p.Id == id).Count = count;
                basket = JsonConvert.SerializeObject(basketVMs);
                HttpContext.Response.Cookies.Append("basket", basket);
                foreach (BasketVM basketVM in basketVMs)
                {
                    basketVM.Title = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Title;
                    basketVM.Image = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).MainImage;
                    basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
                }
                return PartialView("_BasketProductTablePartial", basketVMs);
            }
            else
            {
                return BadRequest();
            }
        }
        public IActionResult RefreshCartProductCount()
        {
            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;
            if (basket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                
                basket = JsonConvert.SerializeObject(basketVMs);
                HttpContext.Response.Cookies.Append("basket", basket);
                foreach (BasketVM basketVM in basketVMs)
                {
                    basketVM.Title = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Title;
                    basketVM.Image = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).MainImage;
                    basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
                }
                return PartialView("_BasketPartial", basketVMs);
            }
            else
            {
                return BadRequest();
            }
        }
        public IActionResult RefreshCartTotal()
        {
            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;
            decimal totalPrice = 0;
            if (basket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id);
                    if (product != null)
                    {
                        basketVM.Title = product.Title;
                        basketVM.Image = product.MainImage;
                        basketVM.Price = product.Price;
                        totalPrice += (basketVM.Count * (decimal)product.Price);
                    }
                }
            }

            return PartialView("_CartTotalPartial",basketVMs);
        }


    }
}

