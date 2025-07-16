using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public required string PictureUrl { get; set; }
        public required string ProductBrand { get; set; }
        public required string ProductType { get; set; }
    }
}
