﻿using System;
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
using Taurus.Models.Formats;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    [Route("vote")]
    public class VoteController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public VoteController(ApplicationContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpPost("VoteCustomer")]
        public async Task<IActionResult> CustomerVote([Bind("CustomerId")] CustomerVote cv)
        {
            if (User.IsInRole("Doctor")) {
                if (await _context.CustomerVotes.Where(m => m.CustomerId == cv.CustomerId).AnyAsync() || await _context.CustomerFlags.Where(m => m.CustomerId == cv.CustomerId).AnyAsync())
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
        public async Task<IActionResult> CustomerFlag([Bind("CustomerId")] CustomerFlag cv)
        {
            if (User.IsInRole("Doctor"))
            {
                if (await _context.CustomerVotes.Where(m => m.CustomerId == cv.CustomerId).AnyAsync() || await _context.CustomerFlags.Where(m => m.CustomerId == cv.CustomerId).AnyAsync())
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

        [HttpPost("VoteDoctor")]
        public async Task<IActionResult> DoctorVote([Bind("DoctorId")] DoctorVote dv)
        {
            if (!User.IsInRole("Doctor"))
            {
                if (await _context.DoctorVotes.Where(m => m.DoctorId == dv.DoctorId).AnyAsync() || await _context.DoctorFlags.Where(m => m.DoctorId == dv.DoctorId).AnyAsync())
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
                }
                else
                {
                    _context.DoctorVotes.Add(dv);
                    await _context.SaveChangesAsync();
                    return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
                }
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpPost("FlagDoctor")]
        public async Task<IActionResult> DoctorFlag([Bind("DoctorId")] DoctorFlag dv)
        {
            if (!User.IsInRole("Doctor"))
            {
                if (await _context.DoctorVotes.Where(m => m.DoctorId == dv.DoctorId).AnyAsync() || await _context.DoctorFlags.Where(m => m.DoctorId == dv.DoctorId).AnyAsync())
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
                }
                else
                {
                    _context.DoctorFlags.Add(dv);
                    await _context.SaveChangesAsync();
                    return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
                }
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }
    }
}
