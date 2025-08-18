using API.DTOs;
using API.DTOs.Identity;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(ICartService cartService, IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
        {
            var email = User.GetEmail();

            var cart = await cartService.GetCartAsync(orderDto.CartId);

            if (cart is null) return BadRequest("Cart not found");

            if (cart.PaymentIntentId is null) return BadRequest("No payment intent for this order");

            var items = new List<OrderItem>();

            foreach (var item in cart.CartItems)
            {
                var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);

                if (productItem is null) return BadRequest("Problem with the order");

                var itemOrdered = new ProductItemOrdered
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    PictureUrl = item.PictureUrl
                };

                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Qty
                };

                items.Add(orderItem);
            }

            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);

            if (deliveryMethod is null) return BadRequest("No delivery method selected");

            var order = new Order
            {
                OrderItems = items,
                DeliveryMethod = deliveryMethod,
                ShippingAddress = orderDto.ShippingAddress,
                Subtotal = items.Sum(x => x.Price * x.Quantity),
                //Discount = orderDto.Discount,
                PaymentSummary = orderDto.PaymentSummary,
                PaymentIntentId = cart.PaymentIntentId,
                BuyerEmail = email
            };

            unitOfWork.Repository<Order>().Add(order);

            if (await unitOfWork.CompleteAsync())
                return order;

            return BadRequest("Problem creating order");
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
        {
            var spec = new OrderWithItemsAndOrderingSpecification(User.GetEmail());

            var orders = await unitOfWork.Repository<Order>().ListAsync(spec);

            var ordersToReturn = orders.Select(mapper.Map<OrderDto>).ToList();

            return Ok(ordersToReturn);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderWithItemsAndOrderingSpecification(User.GetEmail(), id);

            var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order is null) return NotFound();

            var orderToReturn = mapper.Map<OrderDto>(order);
            orderToReturn.OrderItems = order.OrderItems.Select(mapper.Map<OrderItemDto>).ToList();

            return orderToReturn;
        }
    }
}
