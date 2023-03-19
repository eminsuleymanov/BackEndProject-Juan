using System;
using JUANBackendProject.Models;

namespace JUANBackendProject.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
    }
}

