using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;

namespace Taurus.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        // lists
        public virtual List<Session> Sessions { get; set; }
        public virtual List<Question> Questions { get; set; }
        public virtual List<CustomerVote> Votes { get; set; }
        public virtual List<CustomerFlag> Flags { get; set; }
        /* datetime */
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
