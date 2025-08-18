using System.ComponentModel.DataAnnotations;
using API.DTOs.Identity;
using Core.Entities.OrderAggregate;
using Stripe;

namespace API.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public string CartId { get; set; } = string.Empty;

        [Required]
        public ShippingAddress ShippingAddress { get; set; } = null!;

        [Required]
        public PaymentSummary PaymentSummary { get; set; } = null!;

        [Required]
        public int DeliveryMethodId { get; set; }
    }
}
