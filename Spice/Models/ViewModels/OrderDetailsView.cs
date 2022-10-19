using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class OrderDetailsView
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetailsList { get; set; }
    }
}
