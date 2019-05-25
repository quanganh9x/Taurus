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
using Taurus.Models.Enums;

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
                UserName = "Admin",
                Email = "admin@taurus",
                PhoneNumber = "0999999999",
                FullName = "Admin",
                DateOfBirth = DateTime.Parse("1987/01/01"),
                Address = "Admin",
                City = "Hà Nội",
                Country = "Việt Nam",
                Avatar = "http://google.com",
                Gender = Gender.Male,
                Coins = 999999
            };
            await userManager.CreateAsync(user, "Abc/123456");
            await userManager.AddToRoleAsync(user, "Doctor");
            user = new User
            {
                UserName = "Tester",
                Email = "customer@taurus",
                PhoneNumber = "0666666666",
                FullName = "Admin nimdA",
                DateOfBirth = DateTime.Parse("1978/01/01"),
                Address = "nimdA Admin nimdA",
                City = "Hà Nội",
                Country = "Việt Nam",
                Avatar = "http://google.com",
                Gender = Gender.Male,
                Coins = 99999
            };
            await userManager.CreateAsync(user, "Abc/123456");
            await userManager.AddToRoleAsync(user, "Customer");
        }

        public static async Task SeedRole(RoleManager<Role> roleManager)
        {
            await roleManager.CreateAsync(new Role("Doctor"));
            await roleManager.CreateAsync(new Role("VIP"));
            await roleManager.CreateAsync(new Role("Customer"));
            await roleManager.CreateAsync(new Role("Banned"));
        }
    }
}
