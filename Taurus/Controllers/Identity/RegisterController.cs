using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Taurus.Areas.Identity.Models;
using Taurus.Models.Enums;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers.Identity
{
    [AllowAnonymous]
    [Route("Register")]
    public class RegisterController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public RegisterController(SignInManager<User> signInManager, UserManager<User> userManager, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Phone]
            public string PhoneNumber { get; set; }

            [Required]
            public string UserName { get; set; }

            [Required]
            public string FullName { get; set; }

            [Required]
            public string DateOfBirth { get; set; }

            [Required]
            public string Address { get; set; }

            [Required]
            public string City { get; set; }

            [Required]
            public string Country { get; set; }

            [Required]
            public Gender Gender { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        [HttpGet]
        public IActionResult GetRegister(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ReturnUrl = returnUrl;
            return View("~/Views/Identity/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> PostRegister(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User { UserName = Input.UserName, Email = Input.Email, PhoneNumber = Input.PhoneNumber, FullName = Input.FullName, DateOfBirth = DateTime.Parse(Input.DateOfBirth), Address = Input.Address, City = Input.City, Country = Input.Country, Gender = Input.Gender };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}
            }

            // If we got this far, something failed, redisplay form
            return View("~/Views/Identity/Register.cshtml");
        }
    }
}
