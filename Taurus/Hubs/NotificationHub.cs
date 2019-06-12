using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Data;
using Taurus.Models;
using Taurus.Models.Enums;

namespace Taurus.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationContext _context;
        public NotificationHub(ApplicationContext context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            Task.Run(async () => await GetPendingNotifications()).Wait();
            Task.Run(async () => await GetPendingSessions()).Wait();
            return base.OnConnectedAsync();
        }

        public async Task GetPendingNotifications()
        {
            var notifications = await _context.Notifications.Where(s => s.UserId == int.Parse(Context.UserIdentifier)).Select(m => new { Title = m.Title, Description = m.Description, Status = m.Status.ToString(), CreatedAt = m.CreatedAt }).ToListAsync();
            notifications = notifications.TakeLast(7).ToList();
            await Clients.User(Context.UserIdentifier).SendAsync("ReceiveNotifications", Newtonsoft.Json.JsonConvert.SerializeObject(notifications));
        }

        public async Task SetReadNotifications()
        {
            var notifications = await _context.Notifications.Where(s => s.UserId == int.Parse(Context.UserIdentifier) && s.Status == NotificationStatus.UNREAD).ToListAsync();
            if (notifications.Count() > 0)
            {
                foreach (Notification n in notifications)
                {
                    n.Status = NotificationStatus.READ;
                    _context.Notifications.Update(n);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task GetPendingSessions()
        {
            var sessions = await _context.Sessions.Where(s => s.Room.Status != RoomStatus.DONE && s.Customer.UserId == int.Parse(Context.UserIdentifier)).ToListAsync();            
            List <dynamic> ts = new List<dynamic>();
            foreach (Session s in sessions)
            {                
                if (s.Status == SessionStatus.PENDING)
                {
                    ts.Add(new { Message = "You have subscribed to room \"" + s.Room.Title + "\". Your number in the queue is " + (s.Room.Sessions.OrderBy(m => m.Id).ToList().IndexOf(s) - s.Room.Sessions.Count(m => m.Status == SessionStatus.DONE)) + " / " + s.Room.Quota,
                        NumberLeft = (s.Room.Sessions.OrderBy(m => m.Id).ToList().IndexOf(s) - s.Room.Sessions.Count(m => m.Status == SessionStatus.DONE)),
                        Title = s.Room.Title ,Url = "" });
                }
                else if (s.Status == SessionStatus.WAITING)
                {
                    ts.Add(new { Message = "Room \"" + s.Room.Title + "\" is ready for you to join!", Title = s.Room.Title, Url = "/Video/" + s.RoomId });
                }
            }
            await Clients.User(Context.UserIdentifier).SendAsync("ReceiveSessions", Newtonsoft.Json.JsonConvert.SerializeObject(ts));
        }
    }
}
