using System.Collections;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;

namespace WebFramework.Swagger
{
    public class UnauthorizedResponsesOperationFilter : IOperationFilter
    {
        private readonly bool includeUnauthorizedAndForbiddenResponses;
        private readonly OpenApiSecurityScheme schemeName;

        public UnauthorizedResponsesOperationFilter(bool includeUnauthorizedAndForbiddenResponses, OpenApiSecurityScheme schemeName)
        {
            this.includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
            this.schemeName = schemeName;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filters = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            var hasAnonymous = filters.Any(p => p.Filter is AllowAnonymousFilter);
            if (hasAnonymous) return;

            var hasAuthorize = filters.Any(p => p.Filter is AuthorizeFilter);
            if (!hasAuthorize) return;

            if (includeUnauthorizedAndForbiddenResponses)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
            }

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement { { schemeName, new string[] { } } }
            };
        }
    }
}
