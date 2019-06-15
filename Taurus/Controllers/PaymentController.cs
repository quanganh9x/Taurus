using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PayPal.Core;
using PayPal.v1.Payments;
using BraintreeHttp;
using Taurus.Service;
using Taurus.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    [Route("payment")]
    public class PaymentController : Controller
    {
        private readonly Taurus.Data.ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notiService;

        public PaymentController(Taurus.Data.ApplicationContext context, UserManager<User> userManager, INotificationService notiService)
        {
            _context = context;
            _userManager = userManager;
            _notiService = notiService;
        }

        [HttpGet("checkoutfailed")]
        public async Task<IActionResult> CheckoutFailed()
        {            
            return Redirect("/Profile");
        }

        [HttpGet("checkoutsuccess/{amount}")]
        public async Task<IActionResult> CheckoutSuccess(int amount)
        {
            User u = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            u.Coins += amount / 1000;
            await _userManager.UpdateAsync(u);
            return Redirect("/Profile");
        }

        [HttpGet("checkout")]
        public async Task<IActionResult> Checkout(int? amount) {
            if (amount == null)
            {
                return LocalRedirect("/Profile");
            }
            SandboxEnvironment environment = new SandboxEnvironment("Ac1tPYNg6lh1cU6krgaSDv9LikB5ccq6KhtjpCKSkG5dUrTeHj1iBhZ1JQ4vSee0L9ck9AS-mCv4w5VO", "EErMSvuNbHAE8Se3gMHzLS4CX5KCp28fHmWIBaZ5oC1VJIyVGfitPUs8tmEQFSKpX6OYBGNK1qFoLn1h");
            PayPalHttpClient client = new PayPalHttpClient(environment);

            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = ((int)amount/23000).ToString(), // hehe convert xD
                            Currency = "USD"
                        }
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    //CancelUrl = Url.Action("CheckoutFailed", "Payment"),
                    //ReturnUrl = Url.Action("CheckoutSuccess", "Payment")
                    CancelUrl = "https://taurus-infra.azurewebsites.net/payment/checkoutfailed",
                    ReturnUrl = "https://taurus-infra.azurewebsites.net/payment/checkoutsuccess/" + amount
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);

            try
            {
                HttpResponse response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();
                string redirectUrl = null;
                foreach (LinkDescriptionObject link in result.Links)
                {
                    if (link.Rel.Equals("approval_url"))
                    {
                        redirectUrl = link.Href;
                    }
                }

                if (redirectUrl == null)
                {
                    return LocalRedirect("/Profile");
                }
                else
                {
                    return Redirect(redirectUrl);
                }
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                Console.WriteLine("failed " + debugId);
                return LocalRedirect("/Profile");
            }            
        }
    }
}
