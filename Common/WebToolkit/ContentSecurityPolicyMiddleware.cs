using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Common.WebToolkit
{
    public class ContentSecurityPolicyMiddleware
    {
        private const string XFrameOptions = "X-Frame-Options";
        private const string XXssProtection = "X-Xss-Protection";
        private const string XContentTypeOptions = "X-Content-Type-Options";
        private const string ContentSecurityPolicy = "Content-Security-Policy";

        private readonly RequestDelegate _next;
        private readonly string _contentSecurityPolicyValue;
        private readonly IConfiguration _configuration;

        public ContentSecurityPolicyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            _contentSecurityPolicyValue = GetContentSecurityPolicyValue();
        }

        private string GetContentSecurityPolicyValue()
        {
            var contentSecurityPolicyErrorLogUri = _configuration["ContentSecurityPolicyErrorLogUri"];
            if (string.IsNullOrWhiteSpace(contentSecurityPolicyErrorLogUri))
            {
                throw new NullReferenceException("Please set the `ContentSecurityPolicyErrorLogUri` value in `appsettings.json` file.");
            }

            string[] csp =
            {
              "default-src 'self' blob:",
              "style-src 'self' 'unsafe-inline'",
              "script-src 'self' 'unsafe-inline' 'unsafe-eval' ",
              "font-src 'self'",
              "img-src 'self' data: blob:",
              "connect-src 'self'",
              "media-src 'self'",
              "object-src 'self' blob:",
              $"report-uri {contentSecurityPolicyErrorLogUri}"
            };
            return string.Join("; ", csp);
        }

        public Task Invoke(HttpContext context)
        {
            if (!context.Response.Headers.ContainsKey(XFrameOptions))
            {
                context.Response.Headers.Add(XFrameOptions, "SAMEORIGIN");
            }

            if (!context.Response.Headers.ContainsKey(XXssProtection))
            {
                context.Response.Headers.Add(XXssProtection, "1; mode=block");
            }

            if (!context.Response.Headers.ContainsKey(XContentTypeOptions))
            {
                context.Response.Headers.Add(XContentTypeOptions, "nosniff"); // Refused to execute script from '<URL>' because its MIME type ('') is not executable, and strict MIME type checking is enabled.
            }

            if (!context.Response.Headers.ContainsKey(ContentSecurityPolicy))
            {
                context.Response.Headers.Add(ContentSecurityPolicy, _contentSecurityPolicyValue);
            }
            return _next(context);
        }
    }

    public static class ContentSecurityPolicyMiddlewareExtensions
    {
        /// <summary>
        /// Make sure you add this code BEFORE app.UseStaticFiles();,
        /// otherwise the headers will not be applied to your static files.
        /// </summary>
        public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ContentSecurityPolicyMiddleware>();
        }
    }
}