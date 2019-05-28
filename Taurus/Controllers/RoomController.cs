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
    [Route("room")]
    public class RoomController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public RoomController(ApplicationContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNewRoom([Bind("Title,Price")] Room room )
        {           
            room.DoctorId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            room.Status = RoomStatus.PENDING;
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return View();
        }

        [HttpPost("active")]
        public async Task<IActionResult> ActiveRoom([FromForm] int id)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
            r.Status = RoomStatus.ACTIVE;
            await _context.SaveChangesAsync();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("end")]
        public async Task<IActionResult> EndRoom([FromBody] int id)
        {
            var doctorId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == doctorId);
            r.Status = RoomStatus.CLOSED;
            await _context.SaveChangesAsync();
            User u = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            foreach (Session s in r.Sessions)
            {
                u.Coins += s.GetTotalPrice();
            }
            _context.Users.Update(u);
            await _context.SaveChangesAsync();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = r });
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomById(int id) {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            return Ok(room);
        }

        public async Task<IActionResult> UpdateRoomStatus(int id, RoomStatus status)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            room.Status = status;
            return Ok(room);
        }
    }
}
