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
    [Route("note")]
    public class NotesController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public NotesController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        [Route("create")]
        public async Task<IActionResult> CreateNote()
        {
            if (User.IsInRole("Doctor"))
            {
                
            }
            return BadRequest();
        }
    }
}
