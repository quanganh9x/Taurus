using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Taurus.Models;
using Taurus.Models.Enums;

namespace Taurus.Areas.Identity.Models
{
    public class User: IdentityUser<int>
    {
        /* general information */
        [PersonalData, Required]
        public string FullName { get; set; }
        [PersonalData]
        public DateTime DateOfBirth { get; set; }
        [PersonalData]
        public string Address { get; set; }
        [PersonalData]
        public string City { get; set; }
        [PersonalData]
        public string Country { get; set; }
        [PersonalData, Url]
        public string Avatar { get; set; }
        [PersonalData]
        public Gender Gender { get; set; }
        // currencies
        public float Coins { get; set; } = 0;
        // unique
        public string UniqueId { get; set; } = Guid.NewGuid().ToString();

        public int GetAge()
        {
            return (DateTime.Now.Year - this.DateOfBirth.Year);
        }

        public virtual List<CustomerFlag> CustomerFlags { get; set; }
        public virtual List<CustomerVote> CustomerVotes { get; set; }
        public virtual List<DoctorFlag> DoctorFlags { get; set; }
        public virtual List<DoctorVote> DoctorVotes { get; set; }

    }
}
