
using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController(ICartService cartService, IMapper mapper) : BaseApiController
    {

        [HttpGet]
        public async Task<IActionResult> GetCartByIdAsync(string id)
        {
            var cart = await cartService.GetCartAsync(id);

            return Ok(cart ?? new ShoppingCart(id));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartAsync(ShoppingCartDto cartDto)
        {
            var shoppingCart = mapper.Map<ShoppingCartDto, ShoppingCart>(cartDto);

            var updatedcart = await cartService.UpdateCartAsync(shoppingCart);
            if (updatedcart is null) return BadRequest("Problem with cart");

            return Ok(updatedcart);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCartAsync(string id)
        {
            var result = await cartService.DeleteCartAsync(id);
            if (!result) return BadRequest("Problem deleting cart");

            return Ok();
        }
    }
}
