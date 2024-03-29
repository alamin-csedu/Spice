﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class PagingInfo
    {
        public int TotalItem { get; set; }
        public int ItemPerPage { get; set; }
        public int Currentpage { get; set; }
        public int TotalPage => (int) Math.Ceiling((decimal)TotalItem / ItemPerPage);
        public string  urlParam { get; set; }

    }
}
