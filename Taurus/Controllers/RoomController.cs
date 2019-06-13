using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Models;
using Taurus.Models.Enums;
using Taurus.Models.Formats;
using Taurus.Service;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    [Route("room")]
    public class RoomController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notiService;

        public RoomController(ApplicationContext context, UserManager<User> userManager, INotificationService notiService)
        {
            _context = context;
            _userManager = userManager;
            _notiService = notiService;
        }


        // tạo phòng mới
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([Bind("Title,Price,Quota,EstimateTimeStart,EstimateTimeEnd")] Room room)
        {
            if (User.IsInRole("Doctor")) // must be doctor
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User))); 
                room.DoctorId = doctor.Id; 
                room.Status = RoomStatus.PENDING;
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                // save room thành công

                return LocalRedirect("/Video/"+room.Id);
            }
            return LocalRedirect("/");
        }

        // book trước phòng
        [HttpPost("book")]
        public async Task<IActionResult> BookRoom([Bind("Title,Price,Quota")] Room room,  [FromForm] DateTime EstimateTimeStart, DateTime EstimateTimeEnd)
        {            
            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
                room.DoctorId = doctor.Id;
                room.Status = RoomStatus.BOOKED;
                room.EstimateTimeStart = EstimateTimeStart;
                room.EstimateTimeEnd = EstimateTimeEnd;
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                //BackgroundJob.Schedule(
                //() => _notiService.NotifyBookedRoomStartSoon(room),
                //EstimateTimeStart.AddMinutes(-10));

                return Ok(new APIResponse { Status = APIStatus.Success, Data = new {
                                            title = room.Title,
                                            start = room.EstimateTimeStart,
                                            end = room.EstimateTimeEnd
                } });
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpPost("active")]
        public async Task<IActionResult> ActiveRoom([FromForm] int id)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.Doctor.UserId == int.Parse(_userManager.GetUserId(User)) && m.Status != RoomStatus.DONE);
            if (r == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
            }
            if (!r.StartTime.HasValue) // tạo room lần đầu
            {
                r.StartTime = DateTime.Now;
                r.Status = RoomStatus.ACTIVE;
            } else
            {
                r.Status = RoomStatus.WAITING;
            }
            _context.Rooms.Update(r);
            await _context.SaveChangesAsync();
            // update room thành công
            
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestSession([FromForm] int id)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.Doctor.UserId == int.Parse(_userManager.GetUserId(User)) && m.Status != RoomStatus.DONE);
            if (r == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Không tồn tại phòng này" });
            }
            Session remainSession = r.Sessions.FirstOrDefault(m => m.Status == SessionStatus.WAITING);
            if (remainSession != null)
            {
                remainSession.Status = SessionStatus.DONE;
                _context.Sessions.Update(remainSession);
                await _context.SaveChangesAsync();
            }
            List<Session> remainSessions = r.Sessions.FindAll(
                delegate (Session session)
                {
                    return session.Status == SessionStatus.PENDING;
                }
            );
            if (r.Sessions.Count == r.Quota)
            {
                return Ok(new APIResponse { Status = APIStatus.Success, Data = "Hết session" });
            }
            else if (remainSessions.Count == 0)
            {
                return Ok(new APIResponse { Status = APIStatus.Success, Data = "Chưa ai đăng ký zô :)" });
            }
            else
            {
                // chuyển status phòng thành waiting - chờ customer mới
                r.Status = RoomStatus.WAITING;
                _context.Rooms.Update(r);
                Session s = remainSessions.First(); 
                s.Status = SessionStatus.WAITING;
                _context.Sessions.Update(s);
                await _context.SaveChangesAsync();
                var sessions = r.Sessions.Where(m => m.Status == SessionStatus.PENDING || m.Status == SessionStatus.WAITING).ToList();
                foreach (Session sess in sessions)
                {
                    await _notiService.NotifyCustomerTurnUpdate(sess);
                }
            }

            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("update-hb")]
        public async Task<IActionResult> HbUpdateRoom([FromForm] int id)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.Doctor.UserId == int.Parse(_userManager.GetUserId(User)) && m.Status != RoomStatus.DONE);
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
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.Doctor.UserId == int.Parse(_userManager.GetUserId(User)) && m.Status != RoomStatus.DONE);
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
                // prevent UaF
                List<Session> allSessions = r.Sessions;
                foreach (Session s in allSessions)
                {
                    s.Status = SessionStatus.DONE;
                    _context.Sessions.Update(s);
                    await _context.SaveChangesAsync();
                }
                List<Session> sessionInitialized = r.Sessions.FindAll(
                    delegate (Session session)
                    {
                        return (session.Status == SessionStatus.DONE);
                    }
                );
                float temp = 0;
                foreach (Session s in sessionInitialized)
                {
                    u.Coins += s.GetTotalPrice();
                    temp += s.GetTotalPrice();
                }
                _context.Users.Update(u);
                r.Revenue = temp;
                _context.Rooms.Update(r);
                await _context.SaveChangesAsync();
                // notify earning
                await _notiService.NotifyDoctorEarned(r);
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
        public async Task<IActionResult> CheckRoom([FromForm] int id, int sessionId)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id && 
            r.Sessions.FirstOrDefault(s => s.Status == SessionStatus.PENDING).Id == sessionId &&
            r.Sessions.Count(s => s.Status == SessionStatus.PROCESSING) == 0 && r.Status != RoomStatus.DONE); // the only case
            if (room == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
            }
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("addquota")]
        public async Task<IActionResult> AddRoomQuota([FromForm] int id, [FromForm] int Quota)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id && r.Status != RoomStatus.DONE); // the only case
            if (room == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
            }
            room.Quota += Quota;
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
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

        [HttpGet("sessionlist")]
        public async Task<IActionResult> GetSessions(int id)
        {
            if (User.IsInRole("Doctor"))
            {
                Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id && m.Doctor.UserId == int.Parse(_userManager.GetUserId(User)) && m.Status != RoomStatus.DONE);
                if (r == null)
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Có room đâu mà lấy :)" });
                }
                var sessions = r.Sessions.Select(m => new { UserName = m.Customer.User.FullName, Status = m.Status.ToString() }).ToList();
                return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(sessions) });
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Mày không phải Doctor" });
        }
    }
}
