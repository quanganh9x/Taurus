using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;

namespace Taurus.Models
{
    public class ShopItem
    {
        [Key]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public ShopCategory Category { get; set; }
        public string Name { get; set; }
        public string[] Images { get; set; }
        public float Price { get; set; }
        public List<Transaction> Transactions { get; set; }
        /* tracking */
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public ShopItem()
        {
            this.CreatedAt = DateTime.Now;
        }
    }
}
