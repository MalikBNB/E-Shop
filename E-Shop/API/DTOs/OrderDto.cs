using API.DTOs.Identity;
using Core.Entities.OrderAggregate;

namespace API.DTOs
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public AddressDto ShipToAddress { get; set; }
        public int DeliveryMethodId { get; set; }
    }
}
