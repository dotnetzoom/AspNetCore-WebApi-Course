using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.IdentityToolkit;
using Data.Contracts;
using DNTCommon.Web.Core;
using Entities.Identity;
using Entities.User;
using WebFramework.Api;
using Services.Contracts.Identity;
using Services.Identity;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Authorize(Roles = ConstantRoles.Admin)]
    public class RolesController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RolesController> _logger;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IMvcActionsDiscoveryService _mvcActionsDiscoveryService;

        public RolesController(IUserRepository userRepository, ILogger<RolesController> logger, IApplicationRoleManager roleManager, IMvcActionsDiscoveryService mvcActionsDiscoveryService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _roleManager = roleManager;
            _mvcActionsDiscoveryService = mvcActionsDiscoveryService ?? throw new ArgumentNullException(nameof(_mvcActionsDiscoveryService));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(_roleManager));
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult<List<User>>> Get(CancellationToken cancellationToken)
        {
            //var userName = HttpContext.User.Identity.GetUserName();
            //userName = HttpContext.User.Identity.Name;
            //var userId = HttpContext.User.Identity.GetUserId();
            //var userIdInt = HttpContext.User.Identity.GetUserId<int>();
            //var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
            //var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);

            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<DynamicRoleClaimsManagerViewModel>> Get(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var role = await _roleManager.FindRoleIncludeRoleClaimsAsync(id.Value);
            if (role == null)
                return NotFound();

            var securedControllerActions =
                _mvcActionsDiscoveryService.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies
                    .DynamicPermission);

            return new DynamicRoleClaimsManagerViewModel
            {
                SecuredControllerActions = securedControllerActions,
                RoleIncludeRoleClaims = role
            };
        }

        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult> AddOrUpdate(DynamicRoleClaimsManagerViewModel model)
        {
            var result = await _roleManager.AddOrUpdateRoleClaimsAsync(
                model.RoleId,
                ConstantPolicies.DynamicPermissionClaimType,
                model.ActionIds);

            if (!result.Succeeded)
                return BadRequest(result.DumpErrors(true));

            return Ok();
        }
    }
}