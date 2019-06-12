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
        public static User user;

        public static async Task Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            await SeedRole(roleManager);
            await SeedUser(userManager);
        }

        public static async Task SeedUser(UserManager<User> userManager)
        {
            if (await userManager.FindByNameAsync("Admin") == null)
            {
                user = new User
                {
                    UserName = "Admin",
                    Email = "admin@taurus",
                    PhoneNumber = "0999999999",
                    FullName = "Doctor Manhattan",
                    DateOfBirth = DateTime.Parse("1987/01/01"),
                    Address = "12 Ton Duc Thang Street",
                    City = "Ha Noi City",
                    Country = "Viet Nam",
                    Avatar = "https://cdn1.vectorstock.com/i/1000x1000/14/80/doctor-web-icon-therapist-avatar-vector-18531480.jpg",                    
                    Gender = Gender.MALE,
                    Coins = 999999
                };
                await userManager.CreateAsync(user, "Abc/123456");
                await userManager.AddToRoleAsync(user, "Doctor");
            }

            if (await userManager.FindByNameAsync("Doctor") == null)
            {
                user = new User
                {
                    UserName = "Doctor",
                    Email = "DoctorStrange@gmail.com",
                    PhoneNumber = "0997829629",
                    FullName = "Doctor Strange",
                    DateOfBirth = DateTime.Parse("1984/03/08"),
                    Address = "221 Truong Chinh Street",
                    City = "Ho Chi Minh City",
                    Country = "Viet Nam",
                    Avatar = "https://cdn4.iconfinder.com/data/icons/user-avatar-flat-icons/512/User_Avatar-28-512.png",
                    Gender = Gender.MALE,
                    Coins = 1000030
                };
                await userManager.CreateAsync(user, "Abc/123456");
                await userManager.AddToRoleAsync(user, "Doctor");
            }

            if (await userManager.FindByNameAsync("Doctor1") == null)
            {
                user = new User
                {
                    UserName = "Doctor1",
                    Email = "Doctor@gmail.com",
                    PhoneNumber = "0997829629",
                    FullName = "Doctor Who",
                    DateOfBirth = DateTime.Parse("1984/03/08"),
                    Address = "88 Hoang Van Thu Street",
                    City = "Ho Chi Minh City",
                    Country = "Viet Nam",
                    Avatar = "https://cdn1.iconfinder.com/data/icons/diversity-avatars-volume-01-v2/64/doctor-who-eleven-512.png",
                    Gender = Gender.MALE,
                    Coins = 1000030
                };
                await userManager.CreateAsync(user, "Abc/123456");
                await userManager.AddToRoleAsync(user, "Doctor");
            }

            if (await userManager.FindByNameAsync("Tester") == null)
            {
                user = new User
                {
                    UserName = "Tester",
                    Email = "customer@taurus",
                    PhoneNumber = "0666666666",
                    FullName = "Im Tester",
                    DateOfBirth = DateTime.Parse("1978/01/01"),
                    Address = "190 De La Thanh",
                    City = "Hà Nội",
                    Country = "Việt Nam",
                    Avatar = "https://banner2.kisspng.com/20180626/fhs/kisspng-avatar-user-computer-icons-software-developer-5b327cc98b5780.5684824215300354015708.jpg",
                    Gender = Gender.MALE,
                    Coins = 99999
                };
                await userManager.CreateAsync(user, "Abc/123456");
                await userManager.AddToRoleAsync(user, "Customer");
            }

            if (await userManager.FindByNameAsync("Customer1") == null)
            {
                user = new User
                {
                    UserName = "Customer1",
                    Email = "customer1@taurus",
                    PhoneNumber = "0366562170",
                    FullName = "Nguyễn Văn Bệnh",
                    DateOfBirth = DateTime.Parse("1988/11/02"),
                    Address = "Nhà mặt phố",
                    City = "Hà Nội",
                    Country = "Việt Nam",
                    Avatar = "https://banner2.kisspng.com/20180406/gaq/kisspng-computer-icons-avatar-user-profile-people-icon-5ac7ab59364412.4012676915230349692223.jpg",
                    Gender = Gender.MALE,
                    Coins = 99999
                };
                await userManager.CreateAsync(user, "Abc/123456");
                await userManager.AddToRoleAsync(user, "VIP");
            }

            if (await userManager.FindByNameAsync("Customer2") == null)
            {
                user = new User
                {
                    UserName = "Customer2",
                    Email = "customer2@taurus",
                    PhoneNumber = "09304243242",
                    FullName = "Đào Ra Bệnh",
                    DateOfBirth = DateTime.Parse("1989/03/08"),
                    Address = "Biệt Thự ven hồ",
                    City = "Hà Nội",
                    Country = "Việt Nam",
                    Avatar = "https://banner2.kisspng.com/20180403/pde/kisspng-child-computer-icons-avatar-user-avatar-5ac3a1f1da5b25.5067805715227704178944.jpg",
                    Gender = Gender.FEMALE,
                    Coins = 999992
                };
                await userManager.CreateAsync(user, "Abc/123456");
                await userManager.AddToRoleAsync(user, "Customer");
            }

        }

        public static async Task SeedRole(RoleManager<Role> roleManager)
        {
            if (await roleManager.FindByNameAsync("Doctor") == null)
            {
                await roleManager.CreateAsync(new Role("Doctor"));
            }
            if (await roleManager.FindByNameAsync("VIP") == null)
            {
                await roleManager.CreateAsync(new Role("VIP"));
            }
            if (await roleManager.FindByNameAsync("Customer") == null)
            {
                await roleManager.CreateAsync(new Role("Customer"));
            }
            if (await roleManager.FindByNameAsync("Banned") == null)
            {
                await roleManager.CreateAsync(new Role("Banned"));
            }
        }
    }
}
