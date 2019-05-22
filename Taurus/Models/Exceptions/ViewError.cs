using System;

namespace Taurus.Models
{
    public class ViewError
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}