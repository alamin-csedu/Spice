using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class ApplicationUser: IdentityUser
    {
        public String Name { get; set; }
        public String StreetAdress { get; set; }
        public String State { get; set; }
        public String city { get; set; }
        public String PostCode { get; set; }
    }
}
