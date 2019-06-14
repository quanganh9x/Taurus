using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Models.Enums;

namespace Taurus.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public int SessionId { get; set; }
        [ForeignKey("SessionId")]
        public Session Session { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string Medicines { get; set; }
        public string Addition { get; set; }
        public NoteStatus Status { get; set; }
    }
}
