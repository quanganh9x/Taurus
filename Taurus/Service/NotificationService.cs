using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Hubs;
using Taurus.Models;
using Taurus.Models.Enums;

namespace Taurus.Service
{
    public interface INotificationService
    {
        Task NotifyBookedRoomStartSoon(Room room);
        Task NotifyCustomerEnterRoom(Session s);
        Task NotifyCustomerTurnIsReady(Session s);
        Task NotifyDoctorEarned(Room r);
        Task NotifyCustomerConsume(Session s);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<User> _userManager;

        public NotificationService(ApplicationContext context, IHubContext<NotificationHub> hubContext, UserManager<User> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }

        public async Task NotifyBookedRoomStartSoon(Room room) {
            var noti = new Notification(room.Doctor.UserId, "It's about time!", "You have a room named [" + room.Title + "] in the next 10 minutes");
            _context.Notifications.Add(noti);
            _context.SaveChanges();

            await fireNotification(room.Doctor.UserId, "NotificationMessage", noti);
        }

        public async Task NotifyCustomerEnterRoom(Session s)
        {
            //Notification noti = new Notification(s.Room.Doctor.UserId, "New customer entered", "A customer named [" + s.Customer.User.FullName + "] has entered your room");
            //_context.Notifications.Add(noti);
            //await _context.SaveChangesAsync();

            //await fireNotification(s.Room.Doctor.UserId, "NotificationMessage", noti);
        }

        public async Task NotifyCustomerTurnIsReady(Session s) {
            await NotifyPendingSessions(s.Customer.UserId);
        }

        public async Task NotifyCustomerConsume(Session s)
        {
            Notification noti = new Notification(s.Customer.UserId, "Session completed", "You paid " + s.Consume + " to " + s.Room.Doctor.User.FullName);
            _context.Notifications.Add(noti);
            _context.SaveChanges();

            await fireNotification(s.Customer.UserId, "CustomerConsume", noti);
        }

        public async Task NotifyDoctorEarned(Room r)
        {
            Notification noti = new Notification(r.Doctor.UserId, "Room closed", "You earned " + r.Revenue + " with room \"" + r.Title + "\". Hooray!");
            _context.Notifications.Add(noti);
            _context.SaveChanges();

            await fireNotification(r.Doctor.UserId, "DoctorEarned", noti);
        }

        public async Task NotifyPendingNotifications(int userId)
        {
            var notifications = await _context.Notifications.Where(s => s.UserId == userId).Select(m => new { Title = m.Title, Description = m.Description, CreatedAt = m.CreatedAt }).ToListAsync();
            notifications = notifications.TakeLast(7).ToList();
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotifications", Newtonsoft.Json.JsonConvert.SerializeObject(notifications));
        }

        public async Task NotifyPendingSessions(int userId)
        {
            var sessions = await _context.Sessions.Where(s => s.Room.Status != RoomStatus.DONE && s.Customer.UserId == userId).ToListAsync();
            List<dynamic> ts = new List<dynamic>();
            foreach (Session s in sessions)
            {
                if (s.Status == SessionStatus.PENDING)
                {
                    ts.Add(new { Message = "You have subscribed to room [" + s.Room.Title + "]. Your # in the queue is " + s.Room.Sessions.IndexOf(s) + " / " + s.Room.Quota, Url = "" });
                }
                else if (s.Status == SessionStatus.WAITING)
                {
                    ts.Add(new { Message = "Room {" + s.Room.Title + "} is ready for you to join!", Url = "/Video/" + s.RoomId });
                }
            }
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveSessions", Newtonsoft.Json.JsonConvert.SerializeObject(ts));
        }

        private async Task fireNotification(int userId, string eventName, Notification n)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync(eventName, Newtonsoft.Json.JsonConvert.SerializeObject(n));
            await NotifyPendingNotifications(userId);
            await NotifyPendingSessions(userId);
        }
    }
}
