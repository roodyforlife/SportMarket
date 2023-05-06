﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class BasketResponse
    {
        public bool Success { get; set; }
        public bool BasketAdded { get; set; }
        public int Count { get; set; }
    }
}
