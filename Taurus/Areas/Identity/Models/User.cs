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
        [PersonalData]
        public int Age { get; set; }
        [PersonalData]
        public string Address { get; set; }
        [PersonalData]
        public string City { get; set; }
        [PersonalData]
        public string Country { get; set; }
        [PersonalData, Url]
        public string Avatar { get; set; }
    }
}
