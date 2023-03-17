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

            ShopVM shopVM = new ShopVM
            {
                 Products = PageNatedList<Product>.Create(_context.Products.Where(p=>p.IsDeleted==false),pageIndex,3)
            };
            return View(shopVM);
        }
    }
}

