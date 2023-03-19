using System;
using JUANBackendProject.DataAccessLayer;
using JUANBackendProject.Interfaces;
using JUANBackendProject.Models;
using Microsoft.EntityFrameworkCore;

namespace JUANBackendProject.Services
{
    public class LayoutService:ILayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string,string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return settings;
        }
    }
}

