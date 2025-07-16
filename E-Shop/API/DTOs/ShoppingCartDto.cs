using System.ComponentModel.DataAnnotations;
using Core.Entities;

namespace API.DTOs
{
    public class ShoppingCartDto
    {
        [Required]
        public string Id { get; set; } // The client side app is responsible for generating this Id because we are not gonna store it in our Db
        public List<CartItemDto> CartItems { get; set; } = [];
    }
}
