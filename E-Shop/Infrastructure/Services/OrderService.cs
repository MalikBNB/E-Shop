using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public OrderService(IUnitOfWork unitOfWork, ICartService CartService)
        {
            _unitOfWork = unitOfWork;
            _cartService = CartService;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string cartId, ShippingAddress shippingAddress)
        {
            // Get Cart from Redis
            var Cart = await _cartService.GetCartAsync(cartId);

            // Create Order Items
            var items = new List<OrderItem>();
            foreach (var item in Cart.CartItems)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                //var itemOrdered = new ProductOrdered(product.Id, product.Name, product.PictureUrl);
                //var orderItem = new OrderItem(itemOrdered, product.Price, item.Qty);

                //items.Add(orderItem);
            }

            // Get delivery method
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // calc subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            //Create order
            //var order = new Order();
            //_unitOfWork.Repository<Order>().Add(order);

            // Save to db
            var success = await _unitOfWork.CompleteAsync();
            if (!success) return null;

            // Delete Cart
            await _cartService.DeleteCartAsync(cartId);

            return null;
        }

        public async Task<IEnumerable<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrderWithItemsAndOrderingSpecification(buyerEmail, id);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemsAndOrderingSpecification(buyerEmail);

            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}
