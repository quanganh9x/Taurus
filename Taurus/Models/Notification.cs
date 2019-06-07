using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;
using Taurus.Models.Enums;

namespace Taurus.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime Time { get; set; }
        public NotificationStatus Status { get; set; } = NotificationStatus.UNREAD;
        /* datetime */
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public Notification()
        {
        }

        public Notification(int userId, string title, string description, DateTime time)
        {
            UserId = userId;
            Title = title;
            Description = description;
            Time = time;
        }
    }
}
