using System.Linq;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> userManager;

        public UserDataInitializer(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public void InitializeData()
        {
            if (userManager.Users.AsNoTracking().Any(p => p.UserName == "Admin")) return;

            var user = new User
            {
                Age = 25,
                FullName = "محمد جوادابراهیمی",
                Gender = GenderType.Male,
                UserName = "admin",
                Email = "admin@site.com"
            };

            userManager.CreateAsync(user, "123456").GetAwaiter().GetResult();
        }
    }
}