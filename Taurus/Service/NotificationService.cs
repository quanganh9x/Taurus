using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Data;
using Taurus.Hub;
using Taurus.Models;

namespace Taurus.Service
{
    public interface INotificationService
    {
        Task NotifyBookedRoomStartSoon(Room room);
        Task NotifyCustomerEnterRoom(Session s);
        Task NotifyCustomerTurnIsReady(Session s);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(ApplicationContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Tên method hơi củ chuối
        public async Task NotifyBookedRoomStartSoon(Room room) {
            var noti = new Notification(room.Doctor.UserId, "Your customers are waiting!",
                "You have a room named [" + room.Title + "] in the next 10 minutes");
            _context.Notifications.Add(noti);
            _context.SaveChanges();

            await fireNotification(room.Doctor.UserId, "NotificationMessage", noti);
        }

        public async Task NotifyCustomerEnterRoom(Session s)
        {
            Notification noti = new Notification(s.Room.Doctor.UserId, "New customer entered", "A customer named [" + s.Customer.User.FullName + "] has entered your room");
            _context.Notifications.Add(noti);
            await _context.SaveChangesAsync();

            await fireNotification(s.Room.Doctor.UserId, "NotificationMessage", noti);
        }

        public async Task NotifyCustomerTurnIsReady(Session s) {

            Notification noti = new Notification(s.Customer.UserId, "It is your turn!" + s.Room.Id, "You have 30s to join the room named [" + s.Room.Title + "]");
            _context.Notifications.Add(noti);
            _context.SaveChanges();

            await fireNotification(s.Customer.UserId, "BookMarkMessage", noti);
        }

        private async Task fireNotification(int userId, string eventName, Notification n)
        {
            await _hubContext.Clients.User(userId.ToString()).SendAsync("BookmarkMessage", Newtonsoft.Json.JsonConvert.SerializeObject(n));
        }
    }
}
