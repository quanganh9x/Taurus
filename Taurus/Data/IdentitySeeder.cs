using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Models;

namespace Taurus.Data
{
    public class IdentitySeeder
    {
        public static void Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            SeedRole(roleManager);
            Thread.Sleep(5000);
            SeedUser(userManager);
            Thread.Sleep(5000);
        }

        public static void SeedUser(UserManager<User> userManager)
        {
            var user = new User
            {
                UserName = "Administrator",
                Email = "admin@taurus",
                PhoneNumber = "0999999999",
                FullName = "Admin",
                DateOfBirth = DateTime.Parse("1987/01/01"),
                Address = "Admin",
                City = "Hà Nội",
                Country = "Việt Nam",
                Avatar = "http://google.com"
            };
            userManager.CreateAsync(user, "Abc/123456");
            userManager.AddToRoleAsync(user, "Staff");
            Thread.Sleep(5000);
            user = new User
            {
                UserName = "PatientTest",
                Email = "patient@taurus",
                PhoneNumber = "0666666666",
                FullName = "nimdA",
                DateOfBirth = DateTime.Parse("1978/01/01"),
                Address = "nimdA",
                City = "Hà Nội",
                Country = "Việt Nam",
                Avatar = "http://google.com"
            };
            userManager.CreateAsync(user, "Abc/123456");
            userManager.AddToRoleAsync(user, "Patient");
        }

        public static void SeedRole(RoleManager<Role> roleManager)
        {
            roleManager.CreateAsync(new Role("Staff"));
            Thread.Sleep(5000);
            roleManager.CreateAsync(new Role("Patient"));
            Thread.Sleep(5000);
        }
    }
}
