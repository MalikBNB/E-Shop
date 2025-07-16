using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Qty { get; set; }

        [Required]
        public string PictureUrl { get; set; }

        [Required]
        public string ProductBrand { get; set; }

        [Required]
        public string ProductType { get; set; }
    }
}
