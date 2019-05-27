using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models.Enums
{
    public enum RoomStatus
    {
        ACTIVE = 1,
        CLOSED = -1,
        PENDING = 0,
        BUSY = 2
    }
}
