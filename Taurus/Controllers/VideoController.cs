using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        private readonly UserManager<User> _userManager;

        public VideoController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("video/{id}")]
        public async Task<IActionResult> EnterRoom(int id)
        {
            if (User.Identity.Name == null)
            {
                return LocalRedirect("/Login");
            }

            if (User.IsInRole("Doctor"))
            {
                if (await _context.Rooms.Where(m => m.Id == id && m.Doctor.UserId == int.Parse(_userManager.GetUserId(User)) && (m.Status == RoomStatus.PENDING || m.Status == RoomStatus.BOOKED)).AnyAsync())
                {
                    ViewData["User"] = await _context.Doctors.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
                    ViewData["Room"] = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
                    return View("../Room/RoomDoctor");
                }
                return LocalRedirect("/Profile");
            }

            if (await _context.Rooms.Where(m => m.Id == id && (m.Status == RoomStatus.ACTIVE || m.Status == RoomStatus.WAITING)).AnyAsync()) // room trống, đã đăng ký session
            {
                ViewData["User"] = await _context.Customers.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
                ViewData["Room"] = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
                ViewData["Session"] = await _context.Sessions.FirstOrDefaultAsync(m => m.RoomId == id && m.Customer.UserId == int.Parse(_userManager.GetUserId(User)) && (m.Status == SessionStatus.WAITING || m.Status == SessionStatus.PENDING));
                return View("../Room/RoomCustomer");
            }
            return LocalRedirect("/Panel");
        }
    }
}
