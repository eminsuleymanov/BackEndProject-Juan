using System;
using JUANBackendProject.ViewModels.BasketViewModels;

namespace JUANBackendProject.ViewModels.HeaderViewComponentVM
{
    public class HeaderVM
    {
        public IDictionary<string, string> Settings { get; set; }
        public IEnumerable<BasketVM> BasketVMs { get; set; }
    }
}

