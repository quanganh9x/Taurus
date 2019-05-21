﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public int CaseId { get; set; }
        [ForeignKey("CaseId")]
        public virtual Case Case { get; set; }
        public string StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual Staff Staff { get; set; }
        public string PatientId { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        public DateTime Time { get; set; }
        /* status */
        public Status Status { get; set; } = Status.Active;
        /* datetime */
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
