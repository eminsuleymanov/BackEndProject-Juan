using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JUANBackendProject.DataAccessLayer;
using JUANBackendProject.Extensions;
using JUANBackendProject.Helpers;
using JUANBackendProject.Models;
using JUANBackendProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JUANBackendProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Product> products =  _context.Products
                .Where(p => p.IsDeleted == false);
            return View(PageNatedList<Product>.Create(products, pageIndex, 3));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(b => b.IsDeleted == false).ToListAsync();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false ).ToListAsync();

            if (!ModelState.IsValid) return View(product);
            if (!await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", $"Daxil olunan Brand Id {product.BrandId} yanlishdir");
                return View(product);
            }
            if (!await _context.Categories.AnyAsync(b => b.IsDeleted == false && b.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", $"Daxil olunan Category Id {product.CategoryId} yanlishdir");
                return View(product);
            }

            if (product.MainFile != null)
            {
                if (!product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File Jpg olmalidir");
                    return View(product);
                }
                if (!product.MainFile.CheckFileLength(1100))
                {

                    ModelState.AddModelError("MainFile", "Main File 1100kb olmalidir");
                    return View(product);
                }
                product.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "img", "product");

            }
            else
            {
                ModelState.AddModelError("MainFile", "File Mutleq Daxil Olmalidir");
                return View(product);
            }


            if (product.Files == null)
            {
                ModelState.AddModelError("Files", "Wekil mutleq secilmelidir");
                return View(product);
            }

            if (product.Files.Count() > 6)
            {
                ModelState.AddModelError("Files", "Max 6 wekil ola biler");
                return View(product);

            }

            if (product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile file in product.Files)
                {
                    if (!file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Jpg olmalidir");
                        return View(product);
                    }
                    if (!file.CheckFileLength(1100))
                    {

                        ModelState.AddModelError("Files", $"{file.FileName} 1100kb olmalidir");
                        return View(product);
                    }
                    ProductImage productImage = new ProductImage
                    {
                        
                        Image = await file.CreateFileAsync(_env, "assets", "img", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"

                    };
                    productImages.Add(productImage);
                }
                product.ProductImages = productImages;
            }
            product.Title = product.Title.Trim();
            product.CreatedBy = "System";
            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories
                .Where(b => b.IsDeleted == false).ToListAsync();
            
            return View(product);
        }

        

        [HttpGet]
        public async Task<IActionResult> DeleteImageOfProduct(int? id, int? imageId)
        {
            if (id == null) return BadRequest();
            if (imageId == null) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();
            if (product.ProductImages?.Count() <= 1) return BadRequest();

            if (!product.ProductImages.Any(p => p.Id == imageId)) return BadRequest();
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).IsDeleted = true;

            await _context.SaveChangesAsync();
            FileHelper.DeleteFile(product.ProductImages.FirstOrDefault(p => p.Id == imageId).Image, _env, "assets", "img", "product");
            List<ProductImage> productImages = product.ProductImages.Where(p => p.IsDeleted == false).ToList();
            return PartialView("_ProductImagesPartial", productImages);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (id == null) return BadRequest();

            if (id != product.Id) return BadRequest();

            Product dbProduct = await _context.Products
            .Include(pt => pt.ProductImages.Where(p => p.IsDeleted == false))
            .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (dbProduct == null) return NotFound();
            int canUpload = 6 - dbProduct.ProductImages.Count();
            if (product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maksimum {canUpload} qeder şekil yükleye bilersiniz");
                return View(product);
            }
            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (!file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} jpeg tipinde olmalidir!");
                        return View(product);
                    }
                    if (!file.CheckFileLength(1100))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Main File 1100 KB dan cox ola bilmez!");
                        return View(product);
                    }
                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env, "assets", "img", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);

                }

                dbProduct.ProductImages.AddRange(productImages);
            }
            if (product.MainFile != null)
            {
                if (!product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File jpeg tipinde olmalidir!");
                    return View(product);
                }
                if (!product.MainFile.CheckFileLength(1500))
                {
                    ModelState.AddModelError("MainFile", "Main File 1.5 MB-dan cox ola bilmez!");
                    return View(product);
                }
                FileHelper.DeleteFile(dbProduct.MainImage, _env, "assets", "img", "product");
                dbProduct.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "img", "product");
            }

            dbProduct.Title = product.Title;
            dbProduct.Description = product.Description;
            dbProduct.FullDescription = product.FullDescription;
            dbProduct.Price = product.Price;
            dbProduct.DiscountedPrice = product.DiscountedPrice;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow.AddHours(4);
            product.DeletedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}

