using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models.Enums
{
    public enum RoomStatus
    {
        ACTIVE = 1, // bs đã vào phòng
        DONE = -1, // đã kết thúc phiên làm việc
        PENDING = 0, // phòng vừa tạo hoặc đã lên lịch
        PROCESSING = 2, // phòng đang trong phiên làm việc
        BOOKED = 3, // phòng đang chờ tiến trình dequeue,
        WAITING = 4
    }
}
