using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApi.Models;
using Services.Services;
using WebFramework.Api;
// ReSharper disable RedundantOverriddenMember

namespace MyApi.Controllers.v2
{
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class UsersController : v1.UsersController
    {
        public UsersController(IUserRepository userRepository,
            ILogger<v1.UsersController> logger,
            IJwtService jwtService,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signInManager)
            : base(userRepository, logger, jwtService, userManager, roleManager, signInManager)
        {
        }

        public override Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
        {
            return base.Create(userDto, cancellationToken);
        }

        public override Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        public override Task<ActionResult<List<User>>> Get(CancellationToken cancellationToken)
        {
            return base.Get(cancellationToken);
        }

        public override Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        public override Task<ActionResult> Token([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            return base.Token(tokenRequest, cancellationToken);
        }

        public override Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
        {
            return base.Update(id, user, cancellationToken);
        }
    }
}
