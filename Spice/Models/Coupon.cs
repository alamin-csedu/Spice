using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String CouponType { get; set; }
        public enum  ECouponType { Percent=0, Dollar = 1 }
        public double Discount { get; set; }
        public double MinimumAmount { get; set; }
        public byte[] Picture { get; set; }
        public bool isActive { get; set; }
    }
}
