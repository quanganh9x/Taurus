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
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public SessionController(ApplicationContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;        
        }

        /*
            self-developed algorithm:
                0. số session được tạo trước (queued) < quota room
                1. khách subscribes phòng - khởi tạo session, status = PENDING
                2. khách vào phòng - chuyển session ACTIVE
                3. khách out phòng - chuyển session DONE
                4. customer được thông báo khi phòng trống (người khác end 1 session)
                5. thuật toán gale-shapley tiếp tục gọi người tiếp theo
                6. 
         */
        [HttpPost("create")]
        public async Task<IActionResult> CreateSession([Bind("RoomId")] Session s)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == s.RoomId && m.Sessions.Count < m.Quota);
            if (r == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Không tồn tại phòng này" });
            }
            if (await GetTimeRemainingOfUser(r.Price) <= 2) // tối thiểu 2 phút
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Tiền ít đòi hít *** thơm" });
            }
            s.CustomerId = int.Parse(_userManager.GetUserId(User));
            _context.Sessions.Add(s);
            await _context.SaveChangesAsync();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = s.Id });
        }

        [HttpPost("active")]
        public async Task<IActionResult> ActiveSession([FromForm] int id)
        {
            Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == id && m.Status == SessionStatus.PENDING);
            if (s == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Không tồn tại session này" });
            }
            s.Status = SessionStatus.ACTIVE;
            s.StartTime = DateTime.Now;
            _context.Sessions.Update(s);
            await _context.SaveChangesAsync();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("update-hb")]
        public async Task<IActionResult> HbUpdateSession([FromForm] int id)
        {
            Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == id && m.CustomerId == int.Parse(_userManager.GetUserId(User)) && (m.Status == SessionStatus.ACTIVE || m.Status == SessionStatus.PROCESSING));
            if (s == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Không save" });
            }
            s.EndTime = DateTime.Now;
            s.Status = SessionStatus.PROCESSING;
            _context.Sessions.Update(s);
            await _context.SaveChangesAsync();

            int estimatedtime = await GetTimeRemainingOfUser(s.Room.Price);
            if (estimatedtime <= 0) // tối thiểu 2 phút
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Hết giờ rồi bạn hiền" });
            }
            return Ok(new APIResponse { Status = APIStatus.Success, Data = estimatedtime });
        }

        [HttpPost("end")]
        public async Task<IActionResult> EndSession([FromForm] int id)
        {
            Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == id && m.CustomerId == int.Parse(_userManager.GetUserId(User)) && (m.Status == SessionStatus.ACTIVE || m.Status == SessionStatus.PROCESSING));
            if (s == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Không save" });
            }
            s.EndTime = DateTime.Now;
            s.Status = SessionStatus.DONE;
            _context.Sessions.Update(s);
            await _context.SaveChangesAsync();

            try
            {
                User u = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                u.Coins -= s.GetTotalPrice();
                _context.Users.Update(u);
                await _context.SaveChangesAsync(); // holding coins
            } catch (Exception)
            {
                s.Status = SessionStatus.PROCESSING;
                _context.Sessions.Update(s);
                await _context.SaveChangesAsync();
            }

            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [Route("get/{id}")]
        public async Task<IActionResult> GetSessionById(int sessionId)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(session) });
        }

        private async Task<int> GetTimeRemainingOfUser(int Price)
        {
            User u = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            int estimatedtime = (int)Math.Ceiling(u.Coins / Price);
            return estimatedtime;
        }
    }
}
