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

namespace Taurus.Seeders
{
    public class UserSeeder
    {
        public static void SeedUser(UserManager<User> userManager)
        {
            var user = new User
            {
                UserName = "CongCong",
                Email = "congcongdainhan2019@gmail.com",
                PhoneNumber = "0987878888",
                FullName = "Cong",
                DateOfBirth = DateTime.Parse("2019/05/10"),
                Address = "Tử Giám,Giám Tử",
                City = "Hà Nội",
                Country = "Việt Nam",
                Avatar = "http://bizweb.dktcdn.net/100/080/957/files/ap-trung-chim-cong.jpg?v=1486438253113"
            };
            var result = userManager.CreateAsync(user, "Abc/123456").Result;
            if (result.Succeeded)
            {
                Thread.Sleep(9999);
            }
        }

    }
}
