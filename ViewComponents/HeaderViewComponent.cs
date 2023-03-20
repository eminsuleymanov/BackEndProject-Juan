using System;
using JUANBackendProject.DataAccessLayer;
using JUANBackendProject.ViewModels.HeaderViewComponentVM;
using Microsoft.AspNetCore.Mvc;

namespace JUANBackendProject.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(HeaderVM headerVM)
        {

            return View(await Task.FromResult(headerVM));
        }
    }
}

