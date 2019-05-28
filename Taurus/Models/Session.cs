using System;
using System.Collections.Generic;
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
        public RoomStatus Status { get; set; } = RoomStatus.ACTIVE;
        public DateTime CheckTime { get; set; }
        /* datetime */
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public float GetTotalPrice()
        {
            return this.Room.Price * GetTimeSpan();
        }
        
        public int GetTimeSpan()
        {
            return this.CheckTime.Minute - this.CreatedAt.Minute;
        }
    }
}
