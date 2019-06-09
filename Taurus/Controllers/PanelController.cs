using System.Collections.Generic;
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
                return LocalRedirect("/Login");
            }
            if (User.IsInRole("Doctor"))
            {
                return LocalRedirect("/Profile");
            }
            ViewData["Specialists"] = await _context.Specialists.ToListAsync() ;
            ViewData["Facilities"] =  await _context.Facilities.ToListAsync();
            return View("../Panel/Panel");
        }

        [HttpGet("GetListActiveRoom")]
        public async Task<IActionResult> GetListActiveRoom(int? pageIndex)
        {            
            List<Room> listRoom = await _context.Rooms.Where(r => (r.Status != RoomStatus.PENDING && r.Status != RoomStatus.BOOKED && r.Status != RoomStatus.DONE) && r.Sessions.Count < r.Quota).ToListAsync();
            var rooms = new PaginatedList<Room>(listRoom, listRoom.Count(), pageIndex ?? 1, pageSize);            
            return PartialView("/Views/Panel/PanelPartial.cshtml", rooms);
        }

        [HttpGet("GetRoomByCategory/{categoryId}")]
        public async Task<IActionResult> GetRoomByCategory(int categoryId, int? pageIndex)
        {
            List<Room> listRoom = await _context.Rooms.Where(r => (r.Status != RoomStatus.PENDING && r.Status != RoomStatus.BOOKED && r.Status != RoomStatus.DONE) && r.Doctor.Specialist.Id == categoryId).ToListAsync();            
            var rooms = new PaginatedList<Room>(listRoom, listRoom.Count(), pageIndex ?? 1, pageSize);
            return PartialView("/Views/Panel/PanelPartial.cshtml", rooms);
        }

        [HttpGet("GetRoomByFacility/{facilityId}")]
        public async Task<IActionResult> GetRoomByFacility(int facilityId, int? pageIndex)
        {            
            List<Room> listRoom = await _context.Rooms.Where(r => (r.Status != RoomStatus.PENDING && r.Status != RoomStatus.BOOKED && r.Status != RoomStatus.DONE) && r.Doctor.Facility.Id == facilityId).ToListAsync();
            var rooms = new PaginatedList<Room>(listRoom, listRoom.Count(), pageIndex ?? 1, pageSize);
            return PartialView("/Views/Panel/PanelPartial.cshtml", rooms);
        }

    }
}
