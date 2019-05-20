using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models
{
    public class Case
    {
        [Key]
        public int Id { get; set; }
        public string StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual Staff Staff { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public int SpecialistId { get; set; }
        [ForeignKey("SpecialistId")]
        public virtual Specialist Specialist { get; set; }
        /* status */
        public Status Status { get; set; } = Status.Active;
        /* a case has many bills and appointments */
        public virtual List<Bill> Bills { get; set; }
        public virtual List<Appointment> Appointments { get; set; }
        /* datetime */
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
