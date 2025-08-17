using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (userManager.Users.Any()) return;

            var user = new AppUser
            {
                FirstName = "Mohammed",
                LastName = "Bounab",
                Email = "mohammed@email.com",
                UserName = "mohammed@email.com",
                Address = new Address
                {
                    Line1 = "25th street",
                    Line2 = "",
                    City = "Bab-ezzouar",
                    ZipCode = "16000",
                    Country = "Algeria"
                }
            };

            await userManager.CreateAsync(user, "P@ssw0rd");
        }
    }
}
