using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DNTPersianUtils.Core;
using Entities.Identity;
using Entities.Identity.Emails;
using Entities.Identity.Settings;
using Entities.User;
using Microsoft.AspNetCore.Authorization;
using WebFramework.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Services.Contracts.Identity;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    //[Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class PasswordsController : BaseController
    {
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserManager _userManager;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IPasswordValidator<User> _passwordValidator;
        private readonly IOptionsSnapshot<IdentitySiteSettings> _siteOptions;

        public PasswordsController(IApplicationUserManager userManager, IEmailSender emailSender, IOptionsSnapshot<IdentitySiteSettings> siteOptions, IPasswordValidator<User> passwordValidator, IApplicationSignInManager signInManager)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _siteOptions = siteOptions;
            _passwordValidator = passwordValidator;
            _signInManager = signInManager;
        }

        [HttpPost("[action]")]
        public async Task<ApiResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.GetUserAsync(User);

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);

                // reflect the changes in the Identity cookie
                await _signInManager.RefreshSignInAsync(user);

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "اطلاع رسانی تغییر کلمه‌ی عبور",
                    "~/Views/EmailTemplates/_ChangePasswordNotification.cshtml",
                    new ChangePasswordNotificationViewModel
                    {
                        User = user,
                        EmailSignature = _siteOptions.Value.Smtp.FromName,
                        MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                    });

                return Ok();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest();
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ApiResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest();


            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSender.SendEmailAsync(
                    model.Email,
                    "بازیابی کلمه‌ی عبور",
                    "~/Views/EmailTemplates/_PasswordReset.cshtml",
                    new PasswordResetViewModel
                    {
                        UserId = user.Id,
                        Token = code,
                        EmailSignature = _siteOptions.Value.Smtp.FromName,
                        MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                    })
                ;

            return Ok();

        }
    }
}