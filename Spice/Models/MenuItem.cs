using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
        public String Description { get; set; }
        public String Spicyness { get; set; }
        public enum Espicy { NA = 0, NotSpicy = 1, Spicy = 2, VerySpicy = 3 }
        public String Image  { get; set; }
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        [Display(Name = "Sub-Category")]
        public int SubCategoryId { get; set; }
        [Range(1,int.MaxValue,ErrorMessage ="Price should be greater than 1 tk")]
        public double Price { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }
    }
}
