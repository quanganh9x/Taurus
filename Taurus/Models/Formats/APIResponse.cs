using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models.Formats
{
    public class APIResponse
    {
        public APIStatus Status { get; set; }
        public object Data { get; set; }
    }

    public enum APIStatus
    {
        Success = 0,
        Failed = 1
    }

}
