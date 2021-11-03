using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoIdentity.Models
{
    public class CartItem
    {
        public int Quantity { set; get; }
        public Product Product { set; get; }
    }
}
