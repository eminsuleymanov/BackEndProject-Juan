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
using NuGet.ContentModel;

namespace JUANBackendProject.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;
            if (basket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }
            foreach (BasketVM basketVM in basketVMs)
            {
                basketVM.Title = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Title;
                basketVM.Image = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).MainImage;
                basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
            }
            return View(basketVMs);
            
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null) { return BadRequest(); }

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) { return NotFound(); }

            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (string.IsNullOrWhiteSpace(basket))
            {
                basketVMs = new List<BasketVM>
                {
                    new BasketVM {Id = (int)id,Count = 1}
                };
            }
            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                if (basketVMs.Exists(b => b.Id == id))
                {
                    basketVMs.Find(b => b.Id == id).Count += 1;
                }
                else
                {
                    basketVMs.Add(new BasketVM { Id = (int)id, Count = 1 });
                }
            }

            basket = JsonConvert.SerializeObject(basketVMs);

            HttpContext.Response.Cookies.Append("basket", basket);
            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    basketVM.Title = product.Title;
                    basketVM.Image = product.MainImage;
                }
            }
            //int count = basketVMs.Sum(b => b.Count);
            //ViewBag.CartCount = count;


            return PartialView("_BasketPartial", basketVMs);

        }

        public async Task<IActionResult> GetBasket()
        {
            return Json(JsonConvert.DeserializeObject<List<BasketVM>>(HttpContext.Request.Cookies["basket"]));

        }


        [HttpGet]
        public async Task<IActionResult> DeleteFromBasket(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();

            string basket = HttpContext.Request.Cookies["basket"];
            
            if (string.IsNullOrWhiteSpace(basket))
            {
                return BadRequest();               
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            BasketVM basketVM = basketVMs.Find(b => b.Id == id);

            if (basketVM==null) return NotFound();


            basketVMs.Remove(basketVM);
            foreach (var item in basketVMs)
            {
                var product = await _context.Products.FindAsync(item.Id);
                item.Title = product.Title;
                item.Image = product.MainImage;
                item.Price = product.Price;
            }

            basket = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", basket);

            return PartialView("_BasketPartial",basketVMs);
        }

        [HttpGet]
        public async Task<IActionResult> RefreshBasket()
        {
            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                return BadRequest();
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

            foreach (var item in basketVMs)
            {
                var product = await _context.Products.FindAsync(item.Id);
                item.Title = product.Title;
                item.Image = product.MainImage;
                item.Price = product.Price;
            }

            return PartialView("_BasketPartial", basketVMs);
        }

        [HttpGet]
        public async Task<IActionResult> RefreshIndex()
        {
            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                return BadRequest();
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

            foreach (var item in basketVMs)
            {
                var product = await _context.Products.FindAsync(item.Id);
                item.Title = product.Title;
                item.Image = product.MainImage;
                item.Price = product.Price;
            }

            return PartialView("_BasketProductTablePartial", basketVMs);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFromCart(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();

            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                return BadRequest();
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            BasketVM basketVM = basketVMs.Find(b => b.Id == id);

            if (basketVM == null) return NotFound();


            basketVMs.Remove(basketVM);
            foreach (var item in basketVMs)
            {
                var product = await _context.Products.FindAsync(item.Id);
                item.Title = product.Title;
                item.Image = product.MainImage;
                item.Price = product.Price;
            }

            basket = JsonConvert.SerializeObject(basketVMs);
            HttpContext.Response.Cookies.Append("basket", basket);

            return PartialView("_BasketProductTablePartial", basketVMs);

        } 


    }
}

