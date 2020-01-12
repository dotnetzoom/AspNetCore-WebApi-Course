using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using Entities.Identity;
using Entities.Identity.Emails;
using Entities.Identity.Settings;
using Services.Contracts.Identity;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize]
    public class TwoFactorController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<TwoFactorController> _logger;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IOptionsSnapshot<IdentitySiteSettings> _siteOptions;

        public TwoFactorController(
            IApplicationUserManager userManager,
            IApplicationSignInManager signInManager,
            IEmailSender emailSender,
            IOptionsSnapshot<IdentitySiteSettings> siteOptions,
            ILogger<TwoFactorController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(_signInManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(_emailSender));
            _siteOptions = siteOptions ?? throw new ArgumentNullException(nameof(_siteOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        [AllowAnonymous]
        public async Task<ApiResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                return NotFound();


            const string tokenProvider = "Email";
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);

            if (string.IsNullOrWhiteSpace(code))
                return BadRequest();

            await _emailSender.SendEmailAsync(
                               user.Email,
                               "کد جدید اعتبارسنجی دو مرحله‌ای",
                               "~/Views/EmailTemplates/_TwoFactorSendCode.cshtml",
                               new TwoFactorSendCodeViewModel
                               {
                                   Token = code,
                                   EmailSignature = _siteOptions.Value.Smtp.FromName,
                                   MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                               });

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ApiResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(
                model.Provider,
                model.Code,
                model.RememberMe,
                model.RememberBrowser);

            if (result.Succeeded)
                Ok();

            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");

                return BadRequest("Locked out");
            }

            ModelState.AddModelError(string.Empty, "کد وارد شده غیر معتبر است.");

            return BadRequest("Not valid code");
        }
    }
}