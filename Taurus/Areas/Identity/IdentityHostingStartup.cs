using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Taurus.Areas.Identity.Models;
using Taurus.Data;

[assembly: HostingStartup(typeof(Taurus.Areas.Identity.IdentityHostingStartup))]
namespace Taurus.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {

        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
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
                    options.AccessDeniedPath = "/Denied";
                    options.Cookie.Name = "quanganh9x";
                    options.Cookie.Expiration = TimeSpan.FromDays(14);
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    options.LoginPath = "/Login";
                    options.LogoutPath = "/Logout";
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                    options.SlidingExpiration = true;
                });
            });
        }
    }
}