using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using DNTCommon.Web.Core;
using System;
using Entities.Identity.Settings;
using Services.Contracts.Identity;

namespace Services.Identity
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IOptionsSnapshot<SiteSettings> _smtpConfig;
        private readonly IWebMailService _webMailService;

        public AuthMessageSender(
            IOptionsSnapshot<SiteSettings> smtpConfig,
            IWebMailService webMailService)
        {
            _smtpConfig = smtpConfig ?? throw new ArgumentNullException(nameof(_smtpConfig));
            _webMailService = webMailService ?? throw new ArgumentNullException(nameof(webMailService));
        }

        public Task SendEmailAsync<T>(string email, string subject, string viewNameOrPath, T model)
        {
            return _webMailService.SendEmailAsync(
                _smtpConfig.Value.Smtp,
                new[] { new MailAddress { ToName = "", ToAddress = email } },
                subject,
                viewNameOrPath,
                model
            );
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return _webMailService.SendEmailAsync(
                _smtpConfig.Value.Smtp,
                new[] { new MailAddress { ToName = "", ToAddress = email } },
                subject,
                message
            );
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}