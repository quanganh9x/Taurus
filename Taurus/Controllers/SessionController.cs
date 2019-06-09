﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Hub;
using Taurus.Models;
using Taurus.Models.Enums;
using Taurus.Models.Formats;
using Taurus.Service;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    [Route("session")]
    public class SessionController : Controller
    {
        private readonly INotificationService _notiService;
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public SessionController(INotificationService notiService, ApplicationContext context, UserManager<User> userManager)
        {
            _notiService = notiService;
            _context = context;
            _userManager = userManager;
        }


        /*
            1. khách subscribes phòng - khởi tạo session, status = PENDING (số session được tạo trước < quota room)
            2. session được lồng vào Queue
            3. khách out phòng - chuyển session DONE
            4. customer được thông báo khi phòng trống (người khác end 1 session)
            5. thuật toán gale-shapley tiếp tục gọi người tiếp theo
         */
        [HttpPost("create")]
        public async Task<IActionResult> CreateSession([Bind("RoomId")] Session s)
        {
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == s.RoomId && m.Sessions.Count < m.Quota);
            if (r == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Room is not exist or full" });
            }
            if (await GetTimeRemainingOfUser(r.Price) <= 2) // tối thiểu 2 phút
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Your account doesn't have enough coins to join room at least 2 minutes" });
            }
            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
            foreach (Session session in r.Sessions)
            {
                if (session.CustomerId == customer.Id)
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "You have already subscribed this room" });
                }
            }
            List<Session> customerSession = await _context.Sessions.Where(m => m.CustomerId == int.Parse(_userManager.GetUserId(User)) && m.Status == SessionStatus.PENDING).ToListAsync();
            if (customerSession.Count >= 3)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "You can not subscribe more than 3 rooms" });
            }
            s.CustomerId = customer.Id;
            _context.Sessions.Add(s);
            await _context.SaveChangesAsync();
            // session has been created
            return Ok(new APIResponse { Status = APIStatus.Success, Data = s.Id }); // pending session
        }

        /*
         * customer đã thực sự vào room
         */
        [HttpPost("active")]
        public async Task<IActionResult> ActiveSession([FromForm] int id)
        {
            Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == id && m.Customer.UserId == int.Parse(_userManager.GetUserId(User)) && m.Status != SessionStatus.DONE);
            if (s == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Không tồn tại session này" });
            }
            // check lại 1 lần room có tồn tại hoặc có đang trống ?
            Room r = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == s.RoomId && m.Sessions.Contains(s) && m.Status != RoomStatus.DONE);
            if (r == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Room không tồn tại" });
            }
            s.Status = SessionStatus.ACTIVE;
            s.StartTime = DateTime.Now;
            _context.Sessions.Update(s);

            // Notify cho doctor có customer vào phòng  
            await _notiService.NotifyCustomerEnterRoom(s);

            return Ok(new APIResponse { Status = APIStatus.Success, Data = s.Id });
        }

        [HttpPost("update-hb")]
        public async Task<IActionResult> HbUpdateSession([FromForm] int id)
        {
            Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == id && m.Customer.UserId == int.Parse(_userManager.GetUserId(User)) && m.Status != SessionStatus.DONE);
            if (s == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Không save" });
            }
            s.EndTime = DateTime.Now;
            s.Status = SessionStatus.PROCESSING;
            _context.Sessions.Update(s);
            await _context.SaveChangesAsync();

            int estimatedtime = await GetTimeRemainingOfUser(s.Room.Price);
            if (estimatedtime <= 0) // tối thiểu 2 phút
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Hết giờ rồi bạn hiền" });
            }
            return Ok(new APIResponse { Status = APIStatus.Success, Data = estimatedtime });
        }

        [HttpPost("end")]
        public async Task<IActionResult> EndSession([FromForm] int id)
        {
            Session s = await _context.Sessions.FirstOrDefaultAsync(m => m.Id == id && m.Customer.UserId == int.Parse(_userManager.GetUserId(User)));
            if (s == null)
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "Không save" });
            }
            s.EndTime = DateTime.Now;
            s.Status = SessionStatus.DONE;
            _context.Sessions.Update(s);
            await _context.SaveChangesAsync();
            await _notiService.NotifyCustomerTurnIsReady(s.Room.Sessions.FirstOrDefault(m => m.Status == SessionStatus.PENDING));

            try
            {
                User u = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
                u.Coins -= s.GetTotalPrice();
                _context.Users.Update(u);
                await _context.SaveChangesAsync(); // holding coins
            } catch (Exception)
            {
                s.Status = SessionStatus.PROCESSING;
                _context.Sessions.Update(s);
                await _context.SaveChangesAsync();
            }

            return Ok(new APIResponse { Status = APIStatus.Success, Data = null });
        }

        [Route("get/{id}")]
        public async Task<IActionResult> GetSessionById(int id)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id);
            return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(session) });
        }

        private async Task<int> GetTimeRemainingOfUser(int Price)
        {
            User u = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            int estimatedtime = (int)Math.Ceiling(u.Coins / Price);
            return estimatedtime;
        }

        [Route("pending")]
        public async Task<IActionResult> GetPendingSession()
        {
            var sessions = await _context.Sessions.Where(s => (s.Room.Status != RoomStatus.DONE && s.Room.Status != RoomStatus.PENDING && s.Room.Status != RoomStatus.BOOKED) && s.Customer.UserId == int.Parse(_userManager.GetUserId(User))).ToListAsync();
            List<dynamic> ts = new List<dynamic>();
            foreach (Session s in sessions)
            {
                if (s.Status == SessionStatus.PENDING)
                {
                    ts.Add(new { Message = "You have subscribed to room [" + s.Room.Title + "]. Your # in the queue is " + s.Room.Sessions.IndexOf(s) + " / " + s.Room.Quota, Url = "" });
                }
                else if (s.Status == SessionStatus.WAITING)
                {
                    ts.Add(new { Message = "Room {" + s.Room.Title + "} is ready for you to join!", Url = "/Video/" + s.RoomId });
                }
            }
            return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(ts) });
        }
    }
}
