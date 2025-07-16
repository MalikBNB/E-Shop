using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ShoppingCart
    {
        public string Id { get; set; } // The client side app is responsible for generating this Id because we are not gonna store it in our Db
        public List<CartItem> CartItems { get; set; } = [];

        public ShoppingCart() { }

        public ShoppingCart(string id)
        {
            Id = id;
        }


    }
}
