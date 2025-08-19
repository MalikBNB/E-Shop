using API.DTOs;
using API.SignalR;
using AutoMapper;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentService paymentService, IUnitOfWork unitOfWork, 
                                    ILogger<PaymentsController> logger, IConfiguration config, 
                                    IHubContext<NotificationHub> hubContext, IMapper mapper) : BaseApiController
    {
        private readonly string _whSecret = config["StripeSettings:WhSecret"]!;

        [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);
            if (cart is null) return BadRequest("Problem with your cart");

            return Ok(cart);
        }

        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await unitOfWork.Repository<DeliveryMethod>().GetAllAsync());
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = ConstructStripeEvent(json);

                if (stripeEvent.Data.Object is not PaymentIntent intent)
                    return BadRequest("Invalid event data");

                await HandlePaymentIntentSucceeded(intent);

                return Ok();
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, "Stripe webhook error");
                return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
        {
            if (intent.Status == "succeeded")
            {
                var spec = new OrderWithItemsAndOrderingSpecification(intent.Id, true);

                var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec)
                         ?? throw new Exception("Order not found");

                var orderTotalInCents = (long)Math.Round(order.GetTotal() * 100,
                    MidpointRounding.AwayFromZero);

                if (orderTotalInCents != intent.Amount)
                {
                    order.Status = OrderStatus.PaymentMismatch;
                }
                else
                {
                    order.Status = OrderStatus.PaymentRecieved;
                }

                await unitOfWork.CompleteAsync();

                var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

                if (!string.IsNullOrEmpty(connectionId))
                {
                    await hubContext.Clients.Client(connectionId)
                        .SendAsync("OrderCompleteNotification", mapper.Map<OrderDto>(order));
                }
            }
        }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"],
                    _whSecret);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to construct stripe event");
                throw new StripeException("Invalid signature");
            }
        }
    }
}
