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
    public class NotificationService : INotificationService
    {
        private Notification noti;
        private readonly ApplicationContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(ApplicationContext context, IHubContext<NotificationHub> hubContext)
        {
            noti = new Notification();
            _context = context;
            _hubContext = hubContext;
        }

        // Tên method hơi củ chuối
        public void NotifyBookedRoomStartSoon(Room room, int doctorUserId) {
            noti = new Notification(doctorUserId, "Your have a booked room in next 5 minutes",
                "Please click here to open your room [" + room.Title + "]", DateTime.Now);
            _context.Notifications.Add(noti);
            _context.SaveChanges();

            _hubContext.Clients.User(doctorUserId.ToString()).SendAsync("NotificationMessage",
                new
                {
                    Title = noti.Title,
                    Description = noti.Description,
                    Time = noti.Time,
                    Status = noti.Status
                });
        }

        public void NotifyCustomerEnterRoom(Session s)
        {
            Notification noti = new Notification(s.Room.Doctor.UserId, "New customer enter room", "A new customer [" + s.Customer.User.FullName + "] has entered your room", DateTime.Now);
            _context.Notifications.Add(noti);
            _context.SaveChanges();

            _hubContext.Clients.User(s.Room.Doctor.UserId.ToString()).SendAsync("NotificationMessage",
                new
                {
                    Title = noti.Title,
                    Description = noti.Description,
                    Time = noti.Time,
                    Status = noti.Status
                });
        }

        public void NotifyCustomerTurnIsReady(Session s) {

            Notification noti = new Notification(s.Customer.UserId, "Your turn to enter room number " + s.Room.Id, "Please click here in to enter room, you have 30s left", DateTime.Now);
            _context.Notifications.Add(noti);
            _context.SaveChanges();

            _hubContext.Clients.User(s.Customer.UserId.ToString()).SendAsync("BookMarkMessage",
                new
                {
                    Title = noti.Title,
                    Description = noti.Description,
                    Countdown = 30,
                    Time = noti.Time,
                    Status = noti.Status
                });
        }
    }
}
