using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("vote")]
    public class VoteController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public VoteController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("VoteCustomer")]
        public async Task<IActionResult> VoteCustomerQuestion([Bind("CustomerId")] CustomerVote cv)
        {
            if (User.IsInRole("Doctor"))
            {
                if (await _context.CustomerVotes.Where(m => m.CustomerId == cv.CustomerId && m.UserId == int.Parse(_userManager.GetUserId(User))).AnyAsync() || await _context.CustomerFlags.Where(m => m.CustomerId == cv.CustomerId && m.UserId == int.Parse(_userManager.GetUserId(User))).AnyAsync())
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
                }
                else
                {
                    _context.CustomerVotes.Add(cv);
                    await _context.SaveChangesAsync();
                    return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
                }
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpPost("FlagCustomer")]
        public async Task<IActionResult> FlagCustomerQuestion([Bind("CustomerId")] CustomerFlag cv)
        {
            if (User.IsInRole("Doctor"))
            {
                if (await _context.CustomerVotes.Where(m => m.CustomerId == cv.CustomerId && m.UserId == int.Parse(_userManager.GetUserId(User))).AnyAsync() || await _context.CustomerFlags.Where(m => m.CustomerId == cv.CustomerId && m.UserId == int.Parse(_userManager.GetUserId(User))).AnyAsync())
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
                }
                else
                {
                    _context.CustomerFlags.Add(cv);
                    await _context.SaveChangesAsync();
                    return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
                }
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpPost("VoteDoctorAnswer")]
        public async Task<IActionResult> VoteDoctorAnswer([Bind("DoctorId")] DoctorVote dv, [FromForm] int questionId)
        {
            if (!User.IsInRole("Doctor"))
            {
                Question q = await _context.Questions.FirstOrDefaultAsync(m => m.Id == questionId);
                if(q.Status == Status.INACTIVE) return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
                dv.UserId = int.Parse(_userManager.GetUserId(User));
                _context.DoctorVotes.Add(dv);
                q.Status = Status.INACTIVE;
                _context.Questions.Update(q);
                await _context.SaveChangesAsync();
                return Ok(new APIResponse { Status = APIStatus.Success, Data = null });               
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpPost("FlagDoctorAnswer")]
        public async Task<IActionResult> FlagDoctorAnswer([Bind("DoctorId")] DoctorFlag dv, [FromForm] int questionId)
        {
            if (!User.IsInRole("Doctor"))
            {
                Question q = await _context.Questions.FirstOrDefaultAsync(m => m.Id == questionId);
                if (q.Status == Status.INACTIVE) return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
                dv.UserId = int.Parse(_userManager.GetUserId(User));
                _context.DoctorFlags.Add(dv);
                q.Status = Status.INACTIVE;
                _context.Questions.Update(q);
                
                await _context.SaveChangesAsync();
                return Ok(new APIResponse { Status = APIStatus.Success, Data = null });                
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpPost("VoteDoctorRoom")]
        public async Task<IActionResult> VoteDoctorRoom([Bind("DoctorId")] DoctorVote dv, [FromForm] int sessionId)
        {
            if (User.IsInRole("Customer"))
            {
                Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == sessionId);
                if (s.Status == SessionStatus.PENDING) return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
                dv.UserId = int.Parse(_userManager.GetUserId(User));
                _context.DoctorVotes.Add(dv);
                await _context.SaveChangesAsync();
                return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpPost("FlagDoctorRoom")]
        public async Task<IActionResult> FlagDoctorRoom([Bind("DoctorId")] DoctorFlag dv, [FromForm] int sessionId)
        {
            if (User.IsInRole("Customer"))
            {
                Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == sessionId);
                if (s.Status == SessionStatus.PENDING) return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
                dv.UserId = int.Parse(_userManager.GetUserId(User));
                _context.DoctorFlags.Add(dv);
                await _context.SaveChangesAsync();
                return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }
    }
}
