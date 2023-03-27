using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace JUANBackendProject.Models
{
    public class AppUser:IdentityUser
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? SurName { get; set; }

    }
}

