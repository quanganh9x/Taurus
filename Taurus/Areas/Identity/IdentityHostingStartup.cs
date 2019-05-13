using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Taurus.Areas.Identity.Models;
using Taurus.Data;

[assembly: HostingStartup(typeof(Taurus.Areas.Identity.IdentityHostingStartup))]
namespace Taurus.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddIdentity<User, Role>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = false;
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredLength = 0;
                    config.Password.RequiredUniqueChars = 0;
                    config.User.RequireUniqueEmail = false;
                })
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();
                services.ConfigureApplicationCookie(options =>
                {
                    options.AccessDeniedPath = "/";
                    options.Cookie.Name = "quanganh9x";
                    options.ExpireTimeSpan = TimeSpan.MaxValue;
                    options.LoginPath = "/";
                    options.LogoutPath = "/Users/Logout";
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                    options.SlidingExpiration = true;
                });
            });
        }
    }
}