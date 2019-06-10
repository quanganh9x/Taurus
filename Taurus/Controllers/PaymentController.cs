using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PayPal.Core;
using PayPal.v1.Payments;
using BraintreeHttp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ILogger _logger;

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CheckoutFailed(int id)
        {            
            return Redirect("/Profile");
        }

        [HttpGet]
        public async Task<IActionResult> CheckoutSuccess(int id)
        {
            return Redirect("/Profile");
        }

        public async Task<IActionResult> Checkout(int amount) {
            var environment = new SandboxEnvironment("Ac1tPYNg6lh1cU6krgaSDv9LikB5ccq6KhtjpCKSkG5dUrTeHj1iBhZ1JQ4vSee0L9ck9AS-mCv4w5VO", "EErMSvuNbHAE8Se3gMHzLS4CX5KCp28fHmWIBaZ5oC1VJIyVGfitPUs8tmEQFSKpX6OYBGNK1qFoLn1h");
            var client = new PayPalHttpClient(environment);

            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = (amount/23000).ToString(), // hehe convert xD
                            Currency = "USD"
                        }
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = Url.Action("CheckoutFailed", "Payment"),
                    ReturnUrl = Url.Action("CheckoutSuccess", "Payment")
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
                    return RedirectToAction("/Profile");
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
                _logger.LogInformation(statusCode + " - " + debugId);
                return RedirectToAction("/Profile");
            }            
        }
    }
}
