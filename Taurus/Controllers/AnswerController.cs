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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    [Route("answer")]
    public class AnswerController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public AnswerController(ApplicationContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
       
        public async Task<IActionResult> CreateAnswer([Bind("QuestionId", "Text" )] Answer ans)
        {
            if (!User.IsInRole("Doctor")) {
                return BadRequest();
            }
            Doctor d = await _context.Doctors.FirstOrDefaultAsync(m => m.User.Id == Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));

            ans.DoctorId = d.Id;
            _context.Answers.Add(ans);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
