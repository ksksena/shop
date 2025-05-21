using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop
{
    public static class CartManager
    {
        public static List<CartItem> Cart { get; set; } = new List<CartItem>();

        public class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
    }
}