﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class OrderDetailsCart
    {
        public List<ShoppingCart> shoppingCarts { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
