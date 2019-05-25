using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Models.Formats;

namespace Taurus.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _cfg;

        public LoginController(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _cfg = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; } = true;
        }

        // GET: Login
        [HttpGet("/Login")]
        public async Task<IActionResult> GetLogin(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ReturnUrl = returnUrl;
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View("~/Views/Identity/Login.cshtml");
        }

        // POST: Login
        [HttpPost("/Login")]
        public async Task<IActionResult> PostLogin(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("~/Views/Identity/Login.cshtml");
                }
            }

            return View("~/Views/Identity/Login.cshtml");
        }

        [HttpGet("/Logout")]
        [HttpPost("/Logout")]
        public async Task<IActionResult> GetLogout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return LocalRedirect("/");
            }
        }


        // POST: Login
        [HttpPost("/api/Login")]
        public async Task<IActionResult> APILogin()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Input.UserName);

                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, false);
                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Tokens:Key"]));
                        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(_cfg["Tokens:Issuer"],
                          _cfg["Tokens:Audience"],
                          claims,
                          expires: DateTime.Now.AddMinutes(30),
                          signingCredentials: creds);

                        return Ok(new APIResponse { Status = APIStatus.Success, Data = new JwtSecurityTokenHandler().WriteToken(token) });
                    }
                }
            }
            return Ok(new APIResponse { Status = APIStatus.Failed, Data = null });
        }


        // API: Logout
        [HttpPost("/api/Logout")]
        public async Task<IActionResult> APILogout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }
    }
}
