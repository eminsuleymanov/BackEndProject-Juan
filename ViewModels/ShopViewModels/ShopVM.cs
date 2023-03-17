using System;
using JUANBackendProject.Models;

namespace JUANBackendProject.ViewModels.ShopViewModels
{
    public class ShopVM
    {
        public PageNatedList<Product> Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }

    }
}

