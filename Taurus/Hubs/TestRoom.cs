using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Hubs
{
    public class TestRoom: Hub
    {
        public async Task SendRoomEvent(string user, RoomEvent roomEvent, bool isUsingRTC, string Name)
        {
            switch (roomEvent)
            {
                case RoomEvent.ROOM_CREATE:
                    break;
                case RoomEvent.ROOM_FIND:
                    break;
                default:
                    break;
            }
            await Clients.All.SendAsync("Notify", user, "success");
        }
        public async Task AddToRoom()
        {
            Console.WriteLine(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "test");
            await Clients.Group("test").SendAsync("Notify", $"{Context.ConnectionId} has joined.");
        }

        public async Task RemoveFromRoom(string room = "test")
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "test");
            await Clients.Group("test").SendAsync("Notify", $"{Context.ConnectionId} has left.");
        }
    }
}
