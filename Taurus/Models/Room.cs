using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isUsingRTC { get; set; } = true;
        public string Endpoint { get; set; } 
        /* rtc */
        public string[] ICE { get; set; } = new string[2];
        public string[] SDP { get; set; } = new string[2];
        /* tracking */
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public Room()
        {
            this.CreatedAt = DateTime.Now;
        }
    }
}
