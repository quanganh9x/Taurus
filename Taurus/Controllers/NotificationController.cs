using System;
using System.Collections.Generic;
using System.Linq;
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
using Taurus.Service;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    public class NotificationController : Controller
    {
        private readonly int maxNotification = 10;
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notiService;

        public NotificationController(ApplicationContext context, UserManager<User> userManager, INotificationService notiService)
        {
            _context = context;
            _userManager = userManager;
            _notiService = notiService;
        }

       
        public async Task<IActionResult> GetListNotification()
        {
            List<Notification> notifications = new List<Notification>();

            List<Notification> unreadNotis = await _context.Notifications.Where(n => n.UserId == Int32.Parse(_userManager.GetUserId(User)) && n.Status == NotificationStatus.UNREAD).ToListAsync();
            if (unreadNotis.Count() >= maxNotification)
            {
                return Ok(new APIResponse { Status = APIStatus.Success, Data = unreadNotis });
            }
            else {
                notifications.AddRange(unreadNotis);
                List<Notification> readNotis = await _context.Notifications.Where(n => n.UserId == Int32.Parse(_userManager.GetUserId(User)) && n.Status == NotificationStatus.READ).Take(maxNotification - unreadNotis.Count()).ToListAsync();
                notifications.AddRange(readNotis);
                return Ok(new APIResponse { Status = APIStatus.Success, Data = notifications });
            }            
        }

        public async Task<IActionResult> ClearUnreadNotification() {            
            
            List<Notification> unreadNotis = await _context.Notifications.Where(n => n.UserId == Int32.Parse(_userManager.GetUserId(User)) && n.Status == NotificationStatus.UNREAD).ToListAsync();
            List<Notification> readNotis = unreadNotis.Select(n => { n.Status = NotificationStatus.READ; return n; }).ToList();
            _context.UpdateRange(readNotis);
            await _context.SaveChangesAsync();

            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }
    }
}
