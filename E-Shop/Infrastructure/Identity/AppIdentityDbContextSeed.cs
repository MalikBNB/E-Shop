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
                DisplayName = "Mohammed",
                Email = "mohammed@email.com",
                UserName = "mohammed@email.com",
                Address = new Address
                {
                    FirstName = "Mohammed",
                    LastName = "Bounab",
                    Street = "25th street",
                    City = "Algiers",
                    State = "ALG",
                    ZipCode = "16000"
                }
            };

            await userManager.CreateAsync(user, "P@ssw0rd");
        }
    }
}
