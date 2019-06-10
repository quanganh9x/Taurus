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
using Taurus.Models.Formats;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    [Route("profile")]
    public class ProfileController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public ProfileController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("/profile")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Doctor"))
            {
                return View("../Profile/Profile", await _context.Doctors.FirstOrDefaultAsync(p => p.UserId == int.Parse(_userManager.GetUserId(User))));
            }
            else
            {
                ViewData["Sessions"] = await _context.Sessions.Where(m => m.CustomerId == int.Parse(_userManager.GetUserId(User))).ToListAsync();
                return View("../Profile/ProfileCustomer", await _context.Customers.FirstOrDefaultAsync(p => p.UserId == int.Parse(_userManager.GetUserId(User))));
            }
        }

        [HttpGet("/profile/{id}")]
        public async Task<IActionResult> ViewProfile(int id)
        {
            return View("../Profile/ProfileGlobal", await _context.Users.FirstOrDefaultAsync(p => p.Id == id));
        }

        [Route("/pendingSessions")]
        public async Task<IActionResult> GetPendingSessions()
        {
            var sessions = await _context.Sessions.Where(s => (s.Room.Status != RoomStatus.DONE && s.Room.Status != RoomStatus.PENDING && s.Room.Status != RoomStatus.BOOKED) && s.Customer.UserId == int.Parse(_userManager.GetUserId(User))).ToListAsync();
            List<dynamic> ts = new List<dynamic>();
            foreach (Session s in sessions)
            {
                if (s.Status == SessionStatus.PENDING)
                {
                    ts.Add(new { Message = "You have subscribed to room [" + s.Room.Title + "]. Your # in the queue is " + s.Room.Sessions.IndexOf(s) + " / " + s.Room.Quota, Url = "" });
                }
                else if (s.Status == SessionStatus.WAITING)
                {
                    ts.Add(new { Message = "Room {" + s.Room.Title + "} is ready for you to join!", Url = "/Video/" + s.RoomId });
                }
            }
            return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(ts) });
        }

        [Route("/pendingNotifications")]
        public async Task<IActionResult> GetPendingNotifications()
        {
            var notifications = await _context.Notifications.Where(s => s.UserId == int.Parse(_userManager.GetUserId(User))).Select(m => new { Title = m.Title, Description = m.Description, CreatedAt = m.CreatedAt }).ToListAsync();
            notifications = notifications.TakeLast(7).ToList();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(notifications) });
        }

        [HttpPost("edit")]
        public async Task<IActionResult> UpdateProfile([FromForm] string FullName, [FromForm] string Address, [FromForm] string City, [FromForm] string Country, [FromForm] Gender Gender)
        {
            User user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (user == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
            }
            user.FullName = FullName;
            user.Address = Address;
            user.Gender = Gender;
            user.City = City;
            user.Country = Country;
            await _userManager.UpdateAsync(user);
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }
    }
}
