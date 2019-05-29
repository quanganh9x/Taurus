using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;

namespace Taurus.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int SpecialistId { get; set; }
        [ForeignKey("SpecialistId")]
        public virtual Specialist Specialist { get; set; }
        public int FacilityId { get; set; }
        [ForeignKey("FacilityId")]
        public virtual Facility Facility { get; set; }
        /* lists */
        public virtual List<Room> Rooms { get; set; }
        public virtual List<Answer> Answers { get; set; }
        /* analytics */
        public virtual List<DoctorVote> Votes { get; set; }
        public virtual List<DoctorFlag> Flags { get; set; }
        /* datetime */
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
