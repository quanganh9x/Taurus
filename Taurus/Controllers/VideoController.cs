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
using Taurus.Models.Enums;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    public class VideoController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public VideoController(ApplicationContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        
        [HttpGet("video/{id}")]
        public async Task<IActionResult> EnterRoom(int id)
        {
            if (User.IsInRole("Doctor"))
            {
                if (await _context.Rooms.Where(m => m.Id == id && m.DoctorId == int.Parse(_userManager.GetUserId(User)) && m.Status == RoomStatus.PENDING).AnyAsync())
                {
                    ViewData["User"] = await _context.Doctors.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
                    ViewData["Room"] = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
                    return View("../Home/Video");
                }
                return LocalRedirect("/Profile");
            }

            if (await _context.Rooms.Where(m => m.Id == id && m.Status == RoomStatus.ACTIVE).AnyAsync())
            {
                ViewData["User"] = await _context.Customers.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
                ViewData["Room"] = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
                return View("../Home/Video");
            } return LocalRedirect("/Panel");
        }
    }
}
