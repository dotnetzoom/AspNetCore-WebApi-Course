using System;
using System.Linq;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserDataInitializer(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void InitializeData()
        {
            if (_userManager.Users.AsNoTracking().Any(p => p.UserName == "mhkarami97")) return;

            var user = new User
            {
                Birthday = DateTime.Now,
                FullName = "محمد حسین کرمی",
                Gender = GenderType.Male,
                UserName = "mhkarami97",
                Email = "mhkarami1997@gmail.com"
            };

            _userManager.CreateAsync(user, "123456").GetAwaiter().GetResult();

            // var result = _roleManager.FindByNameAsync("Admin");
            // var result1 = _roleManager.FindByNameAsync("User");
            //
            // if (result.Result == null)
            //     _roleManager.CreateAsync(new Role
            //     {
            //         Name = "Admin",
            //         Description = "admin role"
            //     });
            //
            // if (result1.Result == null)
            //     _roleManager.CreateAsync(new Role
            //     {
            //         Name = "User",
            //         Description = "user role"
            //     });
            //
            // _userManager.AddToRoleAsync(user, "Admin");
        }
    }
}