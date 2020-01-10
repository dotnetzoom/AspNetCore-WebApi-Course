using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using DNTCommon.Web.Core;
using Entities.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Services.Contracts.Identity;

namespace Services.Identity
{
    public class DynamicPermissionRequirement : IAuthorizationRequirement
    {
    }

    public class DynamicPermissionsAuthorizationHandler : AuthorizationHandler<DynamicPermissionRequirement>
    {
        private readonly ISecurityTrimmingService _securityTrimmingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DynamicPermissionsAuthorizationHandler(
            ISecurityTrimmingService securityTrimmingService,
            IHttpContextAccessor httpContextAccessor)
        {
            _securityTrimmingService = securityTrimmingService ?? throw new ArgumentNullException(nameof(_securityTrimmingService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override async Task HandleRequirementAsync(
             AuthorizationHandlerContext context,
             DynamicPermissionRequirement requirement)
        {
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();

            var areaName = routeData?.Values["area"]?.ToString();
            var area = string.IsNullOrWhiteSpace(areaName) ? string.Empty : areaName;

            var controllerName = routeData?.Values["controller"]?.ToString();
            var controller = string.IsNullOrWhiteSpace(controllerName) ? string.Empty : controllerName;

            var actionName = routeData?.Values["action"]?.ToString();
            var action = string.IsNullOrWhiteSpace(actionName) ? string.Empty : actionName;

            // How to access form values from an AuthorizationHandler
            var request = _httpContextAccessor.HttpContext.Request;
            if (request.Method.Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                if (request.IsAjaxRequest() && request.ContentType.Contains("application/json"))
                {
                    var httpRequestInfoService = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IHttpRequestInfoService>();
                    var model = await httpRequestInfoService.DeserializeRequestJsonBodyAsAsync<RoleViewModel>();
                    if (model != null)
                    {

                    }
                }
                else
                {
                    foreach (var item in request.Form)
                    {
                        var formField = item.Key;
                        var formFieldValue = item.Value;
                    }
                }
            }

            if (_securityTrimmingService.CanCurrentUserAccess(area, controller, action))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}