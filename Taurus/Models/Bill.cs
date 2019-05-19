using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models
{
    public class Bill
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int CaseId { get; set; }
        [ForeignKey("CaseId")]
        public Case Case { get; set; }
        public string Diagnosis { get; set; }
        public string Note { get; set; }
        /* status */
        public Status Status { get; set; } = Status.Active;
        /* datetime */
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
