using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DNTCommon.Web.Core;
using Services.Contracts.Identity;

namespace Services.Identity
{
    public class SecurityTrimmingService : ISecurityTrimmingService
    {
        private readonly HttpContext _httpContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMvcActionsDiscoveryService _mvcActionsDiscoveryService;

        public SecurityTrimmingService(
            IHttpContextAccessor httpContextAccessor,
            IMvcActionsDiscoveryService mvcActionsDiscoveryService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(_httpContextAccessor));
            _httpContext = _httpContextAccessor.HttpContext;
            _mvcActionsDiscoveryService = mvcActionsDiscoveryService ?? throw new ArgumentNullException(nameof(_mvcActionsDiscoveryService));
        }

        public bool CanCurrentUserAccess(string area, string controller, string action)
        {
            return _httpContext != null && CanUserAccess(_httpContext.User, area, controller, action);
        }

        public bool CanUserAccess(ClaimsPrincipal user, string area, string controller, string action)
        {
            var currentClaimValue = $"{area}:{controller}:{action}";
            var securedControllerActions = _mvcActionsDiscoveryService.GetAllSecuredControllerActionsWithPolicy(ConstantPolicies.DynamicPermission);
            if (!securedControllerActions.SelectMany(x => x.MvcActions).Any(x => x.ActionId == currentClaimValue))
            {
                throw new KeyNotFoundException($"The `secured` area={area}/controller={controller}/action={action} with `ConstantPolicies.DynamicPermission` policy not found. Please check you have entered the area/controller/action names correctly and also it's decorated with the correct security policy.");
            }

            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            if (user.IsInRole(ConstantRoles.Admin))
            {
                // Admin users have access to all of the pages.
                return true;
            }

            // Check for dynamic permissions
            // A user gets its permissions claims from the `ApplicationClaimsPrincipalFactory` class automatically and it includes the role claims too.
            return user.HasClaim(claim => claim.Type == ConstantPolicies.DynamicPermissionClaimType &&
                                          claim.Value == currentClaimValue);
        }
    }
}