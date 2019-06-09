﻿using System;
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
            if (User.IsInRole("Doctor"))
            {
                return View("../Profile/ProfileGlobal", await _context.Doctors.FirstOrDefaultAsync(p => p.UserId == id));
            }
            else
            {
                return View("../Profile/ProfileGlobal", await _context.Customers.FirstOrDefaultAsync(p => p.UserId == id));
            }
        }

        [Route("/api/Profile")]
        [HttpGet]
        public async Task<IActionResult> GetProfileAPI()
        {
            if (User.IsInRole("Doctor"))
            {
                return Ok(new APIResponse { Status = APIStatus.Success, Data = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == int.Parse(_userManager.GetUserId(User))) });
            }
            else
            {
                return Ok(new APIResponse { Status = APIStatus.Success, Data = await _context.Customers.FirstOrDefaultAsync(p => p.Id == int.Parse(_userManager.GetUserId(User))) });
            }
        }
    }
}
