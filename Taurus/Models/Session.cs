using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Models.Enums;

namespace Taurus.Models
{

    public class Session
    {
        [Key]
        public int Id { get; set; }
        public int RoomId { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public SessionStatus Status { get; set; } = SessionStatus.PENDING;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        /* datetime */

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public float GetTotalPrice()
        {
            return this.Room.Price * GetTimeSpan();
        }
        
        public int GetTimeSpan()
        {
            return (this.EndTime - this.StartTime).HasValue ? (int)Math.Ceiling((this.EndTime - this.StartTime).Value.TotalMinutes) : 0;
        }
    }
}
