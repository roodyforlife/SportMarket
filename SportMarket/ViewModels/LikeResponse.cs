using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class LikeResponse
    {
        public bool Success { get; set; }
        public bool? LikeAdded { get; set; }
        public int Count { get; set; }
    }
}
