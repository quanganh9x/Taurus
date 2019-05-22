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
        public static async Task Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (await userManager.FindByNameAsync("Administrator") != null)
            {
                return;
            }
            await SeedRole(roleManager);
            await SeedUser(userManager);
        }

        public static async Task SeedUser(UserManager<User> userManager)
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
                Avatar = "http://google.com",
                Gender = Models.Enums.Gender.Male
            };
            await userManager.CreateAsync(user, "Abc/123456");
            await userManager.AddToRoleAsync(user, "Staff");
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
                Avatar = "http://google.com",
                Gender = Models.Enums.Gender.Male
            };
            await userManager.CreateAsync(user, "Abc/123456");
            await userManager.AddToRoleAsync(user, "Patient");
        }

        public static async Task SeedRole(RoleManager<Role> roleManager)
        {
            await roleManager.CreateAsync(new Role("Staff"));
            await roleManager.CreateAsync(new Role("Patient"));
        }
    }
}
