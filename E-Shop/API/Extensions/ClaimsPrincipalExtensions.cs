using System.Security.Authentication;
using System.Security.Claims;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetEmail(this ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email) ?? throw new AuthenticationException("Email claim not found");

            return email;
        }

        public static async Task<AppUser> GetUserByEmailAsync(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var userToReturn = await userManager.Users.FirstOrDefaultAsync(u => u.Email == user.GetEmail());
            if (userToReturn is null) throw new AuthenticationException("User not found");

            return userToReturn;
        }

        public static async Task<AppUser> GetUserByEmailWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var userToReturn = await userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Email == user.GetEmail());
            if (userToReturn is null) throw new AuthenticationException("User not found");

            return userToReturn;
        }
    }
}
