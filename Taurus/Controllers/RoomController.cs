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

        /*
            1. doctor tạo room với status PENDING
            2. doctor vào room, room chuyển status ACTIVE
            3. room nhận các booked session
         */
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewRoom([Bind("Title,Price,Quota")] Room room)
        {
            if (User.IsInRole("Doctor"))
            {
                room.DoctorId = int.Parse(_userManager.GetUserId(User));
                room.Status = RoomStatus.PENDING;
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                return LocalRedirect("/Video/"+room.Id);
            }
            return LocalRedirect("/");
        }

        [HttpPost("book")]
        public async Task<IActionResult> CreateRoom([Bind("Title,Price,Quota")] Room room,  [FromForm] DateTime EstimatedTimeStart, DateTime EstimatedTimeEnd)
        {            
            if (User.IsInRole("Doctor"))
            {                
                room.DoctorId = int.Parse(_userManager.GetUserId(User));
                room.Status = RoomStatus.BOOKED;
                room.EstimateTimeStart = EstimatedTimeStart;
                room.EstimateTimeEnd = EstimatedTimeEnd;
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                return Ok(new APIResponse { Status = APIStatus.Success, Data = room });
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpPost("active")]
        public async Task<IActionResult> ActiveRoom([FromForm] int id)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == int.Parse(_userManager.GetUserId(User)) && (m.Status == RoomStatus.PENDING || m.Status == RoomStatus.PROCESSING));
            if (r == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
            }
            if (!r.StartTime.HasValue) r.StartTime = DateTime.Now;
            r.Status = RoomStatus.ACTIVE;
            _context.Rooms.Update(r);
            await _context.SaveChangesAsync();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("update-hb")]
        public async Task<IActionResult> HbUpdateRoom([FromForm] int id)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == int.Parse(_userManager.GetUserId(User)) && (m.Status == RoomStatus.ACTIVE || m.Status == RoomStatus.PROCESSING));
            if (r == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
            }
            r.EndTime = DateTime.Now;
            r.Status = RoomStatus.PROCESSING;
            _context.Rooms.Update(r);
            await _context.SaveChangesAsync();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("end")]
        public async Task<IActionResult> EndRoom([FromForm] int id)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == int.Parse(_userManager.GetUserId(User)));
            if (r == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
            }
            r.EndTime = DateTime.Now;
            r.Status = RoomStatus.DONE;
            _context.Rooms.Update(r);
            await _context.SaveChangesAsync();

            try
            {
                User u = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
                List<Session> sessionInitialized = r.Sessions.FindAll(
                    delegate (Session session)
                    {
                        return session.Status == SessionStatus.DONE;
                    }
                );

                foreach (Session s in sessionInitialized)
                {
                    u.Coins += s.GetTotalPrice();
                }
                _context.Users.Update(u);
                await _context.SaveChangesAsync();
            } catch (Exception)
            {
                // oh shit
                r.Status = RoomStatus.PROCESSING;
                _context.Rooms.Update(r);
                await _context.SaveChangesAsync();
            }

            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckRoom([FromForm] int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id && r.Sessions.Count < r.Quota && r.Status == RoomStatus.ACTIVE);
            if (room == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
            }
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetRoomById(int id) {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            return Ok(room);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetRoomsWaiting()
        {
            if (User.IsInRole("Doctor"))
            {
                var Rooms = await _context.Rooms.Where(m => m.DoctorId == int.Parse(_userManager.GetUserId(User)) && m.Status == RoomStatus.BOOKED)
                    .Select(m => new { title = m.Title, start = m.EstimateTimeStart, end = m.EstimateTimeEnd })
                    .ToListAsync();
                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(Rooms));
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Mày không phải Doctor" });
        }

        [HttpGet("stat")]
        public async Task<IActionResult> GetRoomsDone()
        {
            if (User.IsInRole("Doctor"))
            {
                List<Room> Rooms = await _context.Rooms.Where(m => m.DoctorId == int.Parse(_userManager.GetUserId(User)) && m.Status == RoomStatus.DONE).ToListAsync();
                return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(Rooms) });
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Mày không phải Doctor" });
        }

        public async Task<IActionResult> UpdateRoomStatus(int id, RoomStatus status)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            room.Status = status;
            return Ok(room);
        }
    }
}
