using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly int pageSize = 3;

        public PanelController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: /<controller>/      
        [HttpGet()]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            ViewData["Specialists"] = await _context.Specialists.ToListAsync() ;
            ViewData["Facilities"] =  await _context.Facilities.ToListAsync();
            return View("../Panel/Panel");
        }

        [HttpGet("GetListActiveRoom")]
        public async Task<IActionResult> GetListActiveRoom(int? pageIndex)
        {            
            List<Room> listRoom = await _context.Rooms.Where(r => r.Status == RoomStatus.ACTIVE).ToListAsync();
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
