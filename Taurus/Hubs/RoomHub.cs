using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Models;

namespace Taurus.Hubs
{
    public class RoomHub: Hub
    {
        private List<Room> Rooms = new List<Room>(); // rtc rooms
        private List<Room> RoomsSocket = new List<Room>();

        public enum RoomEvent
        {
            ROOM_CREATE = 1,
            ROOM_JOIN = 2,
            ROOM_FIND = 3
        }
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
            await Clients.All.SendAsync("ReceiveMessage", user, "");
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}
