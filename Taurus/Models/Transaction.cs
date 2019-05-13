using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Areas.Identity.Models;

namespace Taurus.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public ShopItem Item { get; set; }
        /* transaction info */
        public int Quantity { get; set; }
        public float Total { get; set; }
        /* tracking */
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public Transaction() {
            this.Total = this.Item.Price * this.Quantity;
            this.CreatedAt = DateTime.Now;
        }
    }
}
