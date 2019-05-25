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
        public DateTime EstimateTimeStart { get; set; }
        public DateTime EstimateTimeEnd { get; set; }
        /* lists */
        public virtual List<Session> Sessions { get; set; }
        // infos
        [Required]
        public string Title { get; set; }
        [Required]
        public int Price { get; set; } // per min
        /* status */
        public RoomStatus Status { get; set; } = RoomStatus.ACTIVE;
        /* datetime */
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
