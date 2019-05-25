using System;
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
    
    [Route("panel")]
    public class PanelController : Controller
    {
        private readonly ApplicationContext _context;

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
        public async Task<IActionResult> GetListActiveRoom()
        {            
            List<Room> rooms = await _context.Rooms.Where(r => r.Status == RoomStatus.ACTIVE).ToListAsync();
            return PartialView("/Views/Panel/RoomPartial.cshtml", rooms);
        }
        
    }
}
