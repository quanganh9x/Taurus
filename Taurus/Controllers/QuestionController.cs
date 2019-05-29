using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taurus.Data;
using Taurus.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    [Route("question")]
    public class QuestionController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionController(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestion()
        {
            var questions = await _context.Questions.Where(q => q.Status == Status.ACTIVE).ToListAsync();
            ViewData["Specialists"] = await _context.Specialists.ToListAsync();
            ViewData["ActiveThreads"] = await _context.Questions.Where(q => q.Status == Status.ACTIVE).OrderBy(m => m.Answers.Count).Take(5).ToListAsync();
            return View("../Home/QA", questions);
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewQuestion([Bind("Title,Text,SpecialistId")] Question q)
        {
            q.CustomerId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _context.Questions.Add(q);
            await _context.SaveChangesAsync();
            return LocalRedirect("/Question");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            ViewData["Specialists"] = await _context.Specialists.ToListAsync();
            ViewData["ActiveThreads"] = await _context.Questions.Where(q => q.Status == Status.ACTIVE && q.CreatedAt > DateTime.Now.AddDays(-1)).OrderBy(m => m.Answers.Count).Take(5).ToListAsync();
            var questions = await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);
            ViewData["Answers"] = await _context.Answers.Where(m => m.QuestionId == id).ToListAsync();
            return View("../Home/Topic", questions);
        }
    }
}
