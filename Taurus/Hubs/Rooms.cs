using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Models;

namespace Taurus.Hubs
{
    public class RoomPlayer
    {
        public string UserName { get; set; } 
        public string Identifier { get; set; } = Guid.NewGuid().ToString();
        public bool isUsingRTC { get; set; } = true;
    }

    public enum RoomEvent
    {
        ROOM_CREATE = 1,
        ROOM_JOIN = 2,
        ROOM_FIND = 3
    }

    public class RoomConfiguration
    {
        public int ExperienceDiffMax = 1000; // max diff exp between 2 players
        public int ExperienceDiffMin = 0; // min diff exp between 2 players
    }

    public class Rooms: Hub
    {
        private List<RoomPlayer> Players = new List<RoomPlayer>(); // rtc-compatible players
        private List<RoomPlayer> LegacyPlayers = new List<RoomPlayer>();

        
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
