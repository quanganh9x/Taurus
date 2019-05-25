using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Models;
using Taurus.Models.Formats;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileController(ApplicationContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("Doctor"))
            {
                return View("../Home/Index");
            }
            
            Doctor doctor = await _context.Doctors.Where(d => d.UserId == Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)).SingleAsync();
            return View("../Home/Profile", doctor);
        }

        public async Task<IActionResult> GetProfile()
        {
            if (User.IsInRole("Doctor"))
            {
                return View(await _context.Doctors.FirstOrDefaultAsync(p => p.Id == int.Parse(_userManager.GetUserId(User))));
            }
            else
            {
                return View(await _context.Customers.FirstOrDefaultAsync(p => p.Id == int.Parse(_userManager.GetUserId(User))));
            }
        }

        [Route("/api/Profile")]
        [HttpGet]
        public async Task<IActionResult> GetProfileAPI()
        {
            if (User.IsInRole("Doctor"))
            {
                return Ok(new APIResponse { Status = APIStatus.Success, Data = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == int.Parse(_userManager.GetUserId(User))) });
            }
            else
            {
                return Ok(new APIResponse { Status = APIStatus.Success, Data = await _context.Customers.FirstOrDefaultAsync(p => p.Id == int.Parse(_userManager.GetUserId(User))) });
            }
        }
    }
}
