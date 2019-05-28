using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public int SpecialistId { get; set; }
        [ForeignKey("SpecialistId")]
        public virtual Specialist Specialist { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        /* lists */
        public virtual List<Answer> Answers { get; set; }
        /* status */
        public Status Status { get; set; } = Status.ACTIVE;
        /* datetime */
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public int GetMinutesPassed()
        {
            return (DateTime.Now - this.CreatedAt).Days;
        }
    }
}
