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

        public async Task<IActionResult> GetAllQuestion()
        {
            var questions = await _context.Questions.Where(q => q.Status == Status.ACTIVE
            && q.CreatedAt > DateTime.Now.AddDays(-1)).ToListAsync();
            return Ok(questions);
        }
        
        public IActionResult CreateNewQuestion([FromForm] string text)
        {
            var question = new Question();
            question.Text = text;
            question.CustomerId = Int32.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _context.Questions.Add(question);
            _context.SaveChanges();
            return View();
        }

        public async Task<IActionResult> GetQuestionById(int id)
        {
            var questions = await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);
            return Ok(questions);
        }
    }
}
