using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public SubCategory subCategory { get; set; }
        public List<String> subCategoryList { get; set; }
        public String ErrorMessage { get; set; }
    }
}
