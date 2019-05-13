using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Models;

namespace Taurus.Areas.Identity.Models
{
    public class User: IdentityUser<int>
    {
        /* general information */
        [PersonalData, Required]
        public string FirstName { get; set; }
        [PersonalData, Required]
        public string LastName { get; set; }
        /* game information */
        public int RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; }
        public float Experience { get; set; } = 0;
        public int Money { get; set; } = 5000;
        public int Point { get; set; } = 0;
        public List<Transaction> Transactions { get; set; }
        /* logging information */
        public DateTime LastLogin { get; set; }
        /* tracking */
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public User()
        {
            this.CreatedAt = DateTime.Now;
        }
    }
}
