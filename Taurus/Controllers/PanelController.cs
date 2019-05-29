﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    [Route("Panel")]
    public class PanelController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly int pageSize = 5;

        public PanelController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /<controller>/      
        [HttpGet()]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.Name == null)
            {
                return LocalRedirect("/");
            }
            if (User.IsInRole("Doctor"))
            {
                Room r = await _context.Rooms.Where(m => m.DoctorId == int.Parse(_userManager.GetUserId(User)) && m.Status == RoomStatus.ACTIVE).FirstOrDefaultAsync();
                if (r == null)
                {
                    return LocalRedirect("/Profile");
                }
                else return LocalRedirect("/Video/" + r.Id);
            }
            ViewData["Specialists"] = await _context.Specialists.ToListAsync() ;
            ViewData["Facilities"] =  await _context.Facilities.ToListAsync();
            return View("../Panel/Panel");
        }

        [HttpGet("GetListActiveRoom")]
        public async Task<IActionResult> GetListActiveRoom(int? pageIndex)
        {            
            List<Room> listRoom = await _context.Rooms.Where(r => r.Status == RoomStatus.ACTIVE && r.Sessions.Count < r.Quota).ToListAsync();
            var rooms = new PaginatedList<Room>(listRoom, listRoom.Count(), pageIndex ?? 1, pageSize);            
            return PartialView("/Views/Panel/RoomPartial.cshtml", rooms);
        }

        [HttpGet("GetRoomByCategory/{categoryId}")]
        public async Task<IActionResult> GetRoomByCategory(int categoryId, int? pageIndex)
        {
            List<Room> listRoom = await _context.Rooms.Where(r => r.Status == RoomStatus.ACTIVE &&
            r.Doctor.Specialist.Id == categoryId).ToListAsync();            
            var rooms = new PaginatedList<Room>(listRoom, listRoom.Count(), pageIndex ?? 1, pageSize);
            return PartialView("/Views/Panel/RoomPartial.cshtml", rooms);
        }

        [HttpGet("GetRoomByFacility/{facilityId}")]
        public async Task<IActionResult> GetRoomByFacility(int facilityId, int? pageIndex)
        {            
            List<Room> listRoom = await _context.Rooms.Where(r => r.Status == RoomStatus.ACTIVE &&
            r.Doctor.Facility.Id == facilityId).ToListAsync();
            var rooms = new PaginatedList<Room>(listRoom, listRoom.Count(), pageIndex ?? 1, pageSize);
            return PartialView("/Views/Panel/RoomPartial.cshtml", rooms);
        }

    }
}
