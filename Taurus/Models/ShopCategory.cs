using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models
{
    public class ShopCategory
    {
        [Key]
        public int Id { get; set; }
        public String Name { get; set; }
        public List<ShopItem> Items { get; set; }
        /* tracking */
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public ShopCategory()
        {
            this.CreatedAt = DateTime.Now;
        }
    }
}
