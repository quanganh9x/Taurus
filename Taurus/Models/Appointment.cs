using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models
{
    public class Appointment
    {
        public int CaseId { get; set; }
        [ForeignKey("CaseId")]
        public Case Case { get; set; }
        public int StaffId { get; set; }
        [ForeignKey("StaffId")]
        public Staff Staff { get; set; }
        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }
        /* status */
        public Status Status { get; set; } = Status.Active;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
