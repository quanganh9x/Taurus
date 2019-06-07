using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models.Enums
{
    public enum SessionStatus
    {
        ACTIVE = 1, // user bắt đầu phiên làm việc với bs
        DONE = -1, // session đã xử lý
        PENDING = 0, // session đang trong queue
        PROCESSING = 2, // user đang trong phiên làm việc với bs,
        WAITING = 3 // trong quá trình dequeue
    }
}
