using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Taurus.Areas.Identity.Models;
using Taurus.Data;
using Taurus.Helper;
using Taurus.Models;
using Taurus.Models.Enums;
using Taurus.Models.Formats;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Taurus.Controllers
{
    [Route("video")]
    public class VideoController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public VideoController(ApplicationContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> EnterRoom(int id)
        {
            if (User.Identity.Name == null)
            {
                return LocalRedirect("/Login");
            }

            if (User.IsInRole("Doctor"))
            {
                if (await _context.Rooms.Where(m => m.Id == id && m.Doctor.UserId == int.Parse(_userManager.GetUserId(User)) && (m.Status == RoomStatus.PENDING || m.Status == RoomStatus.BOOKED)).AnyAsync())
                {
                    ViewData["User"] = await _context.Doctors.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
                    ViewData["Room"] = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
                    if (Utils.fBrowserIsMobile(Request.Headers["User-Agent"].ToString()))
                    {
                        return View("../Mobile/RoomDoctor");
                    }
                    else {
                        return View("../Room/RoomDoctor");
                    }                   
                }
                return LocalRedirect("/Profile");
            }

            if (await _context.Rooms.Where(m => m.Id == id && (m.Status == RoomStatus.ACTIVE || m.Status == RoomStatus.WAITING)).AnyAsync()) // room trống, đã đăng ký session
            {
                ViewData["User"] = await _context.Customers.FirstOrDefaultAsync(m => m.UserId == int.Parse(_userManager.GetUserId(User)));
                ViewData["Room"] = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
                ViewData["Session"] = await _context.Sessions.FirstOrDefaultAsync(m => m.RoomId == id && m.Customer.UserId == int.Parse(_userManager.GetUserId(User)) && (m.Status == SessionStatus.WAITING || m.Status == SessionStatus.PENDING));
                if (Utils.fBrowserIsMobile(Request.Headers["User-Agent"].ToString()))
                {
                    return View("../Mobile/RoomCustomer");
                }
                else
                {
                    return View("../Room/RoomCustomer");
                }
            }
            return LocalRedirect("/Panel");
        }

        [HttpPost("moderation")]
        public async Task<IActionResult> Moderation([FromForm] IFormFile blob)
        {
            if (blob == null || blob.Length == 0) return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "null bleb" });
            var name = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            var stream = blob.OpenReadStream();
            CloudBlockBlob cb = await UploadToBlobStorage(name, stream);
            if (!await cb.ExistsAsync())
            {
                return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "null bleb on storage :/" });
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://api.sightengine.com/1.0/video/check-sync.json?models=nudity,wad&stream_url=" + cb.Uri.AbsoluteUri + "&api_user=" + _configuration["SightEngine:Key"] + "&api_secret=" + _configuration["SightEngine:Secret"]);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                    List<dynamic> frames = obj.data.frames.ToObject<List<dynamic>>();
                    int c = frames.Count();
                    if (c == 0)
                    {
                        return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "cant fetch data" });
                    }
                    float adult = 0, wad = 0, scam = 0;
                    foreach (dynamic frame in frames)
                    {
                        adult += frame.nudity.raw.ToObject<float>();
                        scam += frame.scam.prob.ToObject<float>();
                        wad += (frame.weapon.ToObject<float>() + frame.alcohol.ToObject<float>() + frame.drugs.ToObject<float>()) / 3;
                    }
                    
                    return Ok(new APIResponse { Status = APIStatus.Success, Data = Newtonsoft.Json.JsonConvert.SerializeObject(new { adult = adult / c, wad = wad / c, scam = scam / c }) });
                }
                catch (HttpRequestException)
                {
                    return BadRequest(new APIResponse { Status = APIStatus.Failed, Data = "null bleb" });
                }
            }
        }

        private readonly string FuckIsABlob = "DefaultEndpointsProtocol=https;AccountName=taurusuploads;AccountKey=4c0vjmYVXaMquDRr3yGBcbOXAcvt8YEYRIHsiph5u6yvkaP0AxNMyBZp4fyRSxXT7w0g1ocC5RAlc7I7ddiU+g==;EndpointSuffix=core.windows.net";
        private async Task<CloudBlockBlob> UploadToBlobStorage(string Name, Stream stream)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(FuckIsABlob);
            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Get a reference to a container named "streams"
            CloudBlobContainer container = blobClient.GetContainerReference("streams");
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            // Get a reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(Name);

            // Create or overwrite the "myblob" blob with the contents of a local file
            // named "myfile".
            await blockBlob.UploadFromStreamAsync(stream);
            return blockBlob;
        }
    }
}
