using System;
using JUANBackendProject.Models;

namespace JUANBackendProject.ViewModels.ShopViewModels
{
    public class ShopVM
    {
        public int? CategoryId { get; set; }
        public PageNatedList<Product> Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }

    }
}

