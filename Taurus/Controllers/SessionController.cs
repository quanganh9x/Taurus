using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taurus.Data;
using Taurus.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    public class SessionController : Controller
    {
        private readonly ApplicationContext _context;        

        public SessionController(ApplicationContext context)
        {
            _context = context;           
        }

        // GET: /<controller>/
        public IActionResult CreateNewSession([FromBody] int roomId, int customerId)
        {
            var session = new Session();
            session.CustomerId = customerId;
            session.RoomId = roomId;
            _context.Sessions.Add(session);
            _context.SaveChanges();
            return View();
        }

        public async Task<IActionResult> EndSession(int sessionId)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            session.Status = Status.DONE;
            _context.SaveChanges();

            // Simple calculate cost for video call (update later)
            var callTime = (session.UpdatedAt - session.CreatedAt).TotalMinutes;
            var cost = callTime * session.Room.Price;
            session.Customer.User.Coins -= (float) cost;

            return Ok(session.Status);
        }

        public async Task<IActionResult> GetSessionById(int sessionId)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);            
            return Ok(session);
        }
    }
}
