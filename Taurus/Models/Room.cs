using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Taurus.Models.Enums;

namespace Taurus.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
        public DateTime? EstimateTimeStart { get; set; }
        public DateTime? EstimateTimeEnd { get; set; }
        /* lists */
        public virtual List<Session> Sessions { get; set; }
        // infos
        public string Title { get; set; }
        public int Price { get; set; } // per min
        public int Quota { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        /* analytics */
        public float Revenue { get; set; }
        /* status */
        public RoomStatus Status { get; set; } = RoomStatus.PENDING;
        /* datetime */
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int GetTimeSpan()
        {
            return (this.EndTime - this.StartTime).HasValue ? (int)Math.Ceiling((this.EndTime - this.StartTime).Value.TotalMinutes) : 0;
        }
        public float GetRevenue()
        {
            float temp = 0;
            foreach(Session s in this.Sessions)
            {
                temp += s.GetTotalPrice();
            }
            return temp;
        }
    }
}
