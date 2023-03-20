using System;
using JUANBackendProject.Models;
using JUANBackendProject.ViewModels.BasketViewModels;

namespace JUANBackendProject.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<BasketVM>> GetBaskets();
    }
}

