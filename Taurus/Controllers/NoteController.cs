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
    [Route("note")]
    public class NoteController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public NoteController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateNote([FromForm] string[] symptoms, string[] diagnosis, string[] medicines, string addition, int status, int sessionId)
        {
            if (User.IsInRole("Doctor"))
            {
                Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == sessionId && m.Status != SessionStatus.PENDING);
                if (s == null)
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "null session ?" });
                }
                Note n = new Note { SessionId = sessionId, Symptoms = Newtonsoft.Json.JsonConvert.SerializeObject(symptoms), Medicines = Newtonsoft.Json.JsonConvert.SerializeObject(medicines), Diagnosis = Newtonsoft.Json.JsonConvert.SerializeObject(diagnosis), Addition = addition, Status = (NoteStatus)status };
                _context.Notes.Add(n);
                await _context.SaveChangesAsync();
                s.NoteId = n.Id;
                _context.Sessions.Update(s);
                await _context.SaveChangesAsync();
                return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpGet("getNotes")]
        public async Task<IActionResult> GetNotes(int userId)
        {
            if (User.IsInRole("Doctor"))
            {
                Customer c = await _context.Customers.FirstOrDefaultAsync(m => m.UserId == userId);
                if (c == null)
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "null customer" });
                }
                var notes = new List<dynamic>();
                foreach (Session s in c.Sessions.Where(m => m.Status == SessionStatus.DONE && m.Note != null))
                {
                    notes.Add(new { SessionId = s.Id, Symptoms = Newtonsoft.Json.JsonConvert.SerializeObject(s.Note.Symptoms), Medicines = Newtonsoft.Json.JsonConvert.SerializeObject(s.Note.Medicines), Diagnosis = Newtonsoft.Json.JsonConvert.SerializeObject(s.Note.Diagnosis), Addition = s.Note.Addition, Status = s.Note.Status });
                }
                return Ok(new APIResponse { Status = APIStatus.Success, Data = notes });
            }
            return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = null });
        }

        [HttpGet("getNote")]
        public async Task<IActionResult> GetNote(int sessionId)
        {
            Session session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.Status == SessionStatus.DONE && s.NoteId != 0);            
            if (session == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Session doesn't have note" });
            }
            if (_userManager.GetUserId(User) != session.Customer.UserId.ToString() && _userManager.GetUserId(User) != session.Room.Doctor.UserId.ToString())
            {
                return Unauthorized(new APIResponse { Status = APIStatus.Failed, Data = "You dont have permission to see this note" });
            }
            return Ok(new APIResponse
            {
                Status = APIStatus.Success,
                Data = new { Symptoms = session.Note.Symptoms, Medicines = session.Note.Medicines, Diagnosis = session.Note.Diagnosis, Addition = session.Note.Addition, Status = session.Note.Status }
            });
        }
    }
}
