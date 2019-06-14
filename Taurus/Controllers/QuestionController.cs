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
    [Route("question")]
    public class QuestionController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public QuestionController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllQuestion()
        {
            if (User.Identity.Name == null)
            {
                return LocalRedirect("/Login");
            }
            var questions = await _context.Questions.ToListAsync();
            ViewData["Specialists"] = await _context.Specialists.ToListAsync();
            ViewData["ActiveThreads"] = await _context.Questions.Where(q => q.Status == QuestionStatus.ACTIVE).OrderBy(m => m.Answers.Count).Take(5).ToListAsync();
            return View("../Ask/Index", questions);
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateNewQuestion([Bind("Title,Text,SpecialistId")] Question q)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
            q.CustomerId = customer.Id;
            _context.Questions.Add(q);
            await _context.SaveChangesAsync();
            return LocalRedirect("/Question");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            ViewData["Specialists"] = await _context.Specialists.ToListAsync();
            ViewData["ActiveThreads"] = await _context.Questions.Where(q => q.Status == QuestionStatus.ACTIVE && q.CreatedAt > DateTime.Now.AddDays(-1)).OrderBy(m => m.Answers.Count).Take(5).ToListAsync();
            var questions = await _context.Questions.FirstOrDefaultAsync(q => q.Id == id);
            ViewData["Answers"] = await _context.Answers.Where(m => m.QuestionId == id).ToListAsync();
            return View("../Ask/Topic", questions);
        }
    }
}
