using System.Security.Claims;
using API.DTOs.Identity;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<AppUser> userManager,
                                    SignInManager<AppUser> signInManager,
                                    ITokenService tokenService,
                                    IMapper mapper) : BaseApiController
    {

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
                return new BadRequestObjectResult
                (
                    new ApiValidationErrorResponse
                    {
                        Errors = new[]
                        {
                            "Email adress is in use"
                        }
                    }
                );

            var user = new AppUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem();
            }

            return new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenService.CreateToken(user)
            };
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if ((bool)!User.Identity?.IsAuthenticated) return NoContent();

            var user = await signInManager.UserManager.FindUserByClaimsPrincipleWithAdressAsync(User);
            return Ok(new
            {
                user.FirstName,
                user.LastName,
                user.Email,
                Address = mapper.Map<Address, AddressDto>(user.Address)
        });
        }

        [HttpGet]
        public ActionResult GetAuthState()
        {
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await userManager.FindByEmailFromClaimsPricipleAsync(HttpContext.User);

            return new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenService.CreateToken(user)
            };
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await userManager.FindUserByClaimsPrincipleWithAdressAsync(HttpContext.User);

            return mapper.Map<Address, AddressDto>(user.Address);
        }

        [HttpGet("email-exists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync(string email)
        {
            return await userManager.FindByEmailAsync(email) is not null;
        }

        [Authorize]
        [HttpPost("address")]
        public async Task<ActionResult<AddressDto>> CreateOrUpdateUserAddress(AddressDto addressDto)
        {
            var user = await signInManager.UserManager.GetUserByEmailWithAddressAsync(User);
            user.Address = mapper.Map<AddressDto, Address>(addressDto);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest("Faild updating the user");

            return Ok(mapper.Map<Address, AddressDto>(user.Address));
        }

        //[HttpPost("login")]
        //public async Task<ActionResult<UserDto>> LogIn(LoginDto loginDto)
        //{
        //    var user = await userManager.FindByEmailAsync(loginDto.Email);
        //    if (user is null) return Unauthorized(new ApiResponse(401));

        //    var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        //    if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

        //    return new UserDto
        //    {
        //        Email = user.Email,
        //        DisplayName = user.DisplayName,
        //        Token = tokenService.CreateToken(user)
        //    };
        //}


    }
}
