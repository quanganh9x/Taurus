using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Hubs
{
    public class VideoRoomHub: Hub
    {
        private readonly ILogger _logger;

        public VideoRoomHub(ILogger<VideoRoomHub> logger)
        {
            _logger = logger;
        }
        public async Task Send(string name, string message)
        {
            _logger.LogDebug("event: " + name + " // " + message);
            await Clients.All.SendAsync("Receive", name, message);

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(message);

            switch (json.eventName) {
                case "open-room":
                    break;
                default:
                    break;
            }
        }


    }
}
