using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Models;

namespace Taurus.Service
{
    public interface INotificationService
    {
        void NotifyBookedRoomStartSoon(Room room, int doctorUserId);
        void NotifyCustomerEnterRoom(Session s);
    }
}
