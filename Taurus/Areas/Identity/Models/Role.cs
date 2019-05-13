using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taurus.Areas.Identity.Models
{
    public class Role: IdentityRole<int>
    {
        public Role(string name)
        {
            this.Name = name;
        }
    }
}
