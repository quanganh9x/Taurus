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

        // GET: /<controller>/
        [HttpPost("create")]
        public async Task<IActionResult> CreateSession([Bind("RoomId")] Session s)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == s.RoomId);
            s.CustomerId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User u = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            int estimatedtime = (int)Math.Ceiling(u.Coins / r.Price);
            if (estimatedtime <= 0)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Tiền ít đòi hít *** thơm" });
            }
            _context.Sessions.Add(s);
            await _context.SaveChangesAsync();
            return Ok(new APIResponse { Status = APIStatus.Success, Data = estimatedtime });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateSession()
        {
            var customerId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.CustomerId == customerId && m.Status == RoomStatus.ACTIVE);
            if (s == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "T k save" });
            }
            s.CheckTime = DateTime.Now;
            _context.Sessions.Update(s);
            await _context.SaveChangesAsync();
            User u = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            u.Coins -= s.GetTotalPrice();
            _context.Users.Update(u);
            await _context.SaveChangesAsync(); // holding coins
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [HttpPost("end")]
        public async Task<IActionResult> EndSession()
        {
            var customerId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.CustomerId == customerId && m.Status == RoomStatus.ACTIVE);
            if (s == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "T k save" });
            }
            //s.CheckTime = DateTime.Now;
            s.Status = RoomStatus.CLOSED;
            _context.Sessions.Update(s);
            await _context.SaveChangesAsync();
            User u = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            u.Coins -= s.GetTotalPrice();
            _context.Users.Update(u);
            await _context.SaveChangesAsync(); // holding coins
            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [Route("get/{id}")]
        public async Task<IActionResult> GetSessionById(int sessionId)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(session) });
        }
    }
}
