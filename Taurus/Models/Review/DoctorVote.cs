using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;

namespace Taurus.Models
{
    public class DoctorVote
    {
        [Key]
        public int Id { get; set; }
        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User Origin { get; set; }
        /* datetime */

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
