using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;

namespace Taurus.Models
{
    public class Staff
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int SpecialistId { get; set; }
        [ForeignKey("SpecialistId")]
        public Specialist Specialist { get; set; }
        public int FacilityId { get; set; }
        [ForeignKey("FacilityId")]
        public Facility Facility { get; set; }
        /* status */
        public Status Status { get; set; } = Status.Active;
        /* one staff has many cases & appointments */
        public List<Case> Cases { get; set; }
        public List<Appointment> Appointments { get; set; }
        /* datetime */
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
