﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Models
{
    public class Specialist
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        /* lists */
        public virtual List<Doctor> Doctors { get; set; }
        public virtual List<Question> Questions { get; set; }
        /* datetime */
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
