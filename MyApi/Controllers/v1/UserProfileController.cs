using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using System;
using Common.GuardToolkit;
using Common.IdentityToolkit;
using DNTCommon.Web.Core;
using Entities.Identity;
using Entities.Identity.Emails;
using Entities.Identity.Settings;
using Entities.User;
using Services.Contracts.Identity;
using Services.Identity;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IProtectionProviderService _protectionProviderService;
        private readonly IApplicationRoleManager _roleManager;
        private readonly IApplicationSignInManager _signInManager;
        private readonly IOptionsSnapshot<SiteSettings> _siteOptions;
        private readonly IUsedPasswordsService _usedPasswordsService;
        private readonly IApplicationUserManager _userManager;
        private readonly IUsersPhotoService _usersPhotoService;
        private readonly IUserValidator<User> _userValidator;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            IApplicationUserManager userManager,
            IApplicationRoleManager roleManager,
            IApplicationSignInManager signInManager,
            IProtectionProviderService protectionProviderService,
            IUserValidator<User> userValidator,
            IUsedPasswordsService usedPasswordsService,
            IUsersPhotoService usersPhotoService,
            IOptionsSnapshot<SiteSettings> siteOptions,
            IEmailSender emailSender,
            ILogger<UserProfileController> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(_roleManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(_signInManager));
            _protectionProviderService = protectionProviderService ?? throw new ArgumentNullException(nameof(_protectionProviderService));
            _userValidator = userValidator ?? throw new ArgumentNullException(nameof(_userValidator));
            _usedPasswordsService = usedPasswordsService ?? throw new ArgumentNullException(nameof(_usedPasswordsService));
            _usersPhotoService = usersPhotoService ?? throw new ArgumentNullException(nameof(_usersPhotoService));
            _siteOptions = siteOptions ?? throw new ArgumentNullException(nameof(_siteOptions));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(_emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        [Authorize(Roles = ConstantRoles.Admin)]
        public async Task<ApiResult<UserProfileViewModel>> AdminEdit(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id.ToString());

            return await RenderForm(user, true);
        }

        public async Task<ApiResult<UserProfileViewModel>> Get()
        {
            var user = await _userManager.GetCurrentUserAsync();

            return await RenderForm(user, false);
        }

        [HttpPut]
        public async Task<ApiResult<UserProfileViewModel>> Update(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pid = _protectionProviderService.Decrypt(model.Pid);

                if (string.IsNullOrWhiteSpace(pid))
                    return BadRequest();


                if (pid != _userManager.GetCurrentUserId() &&
                    !_roleManager.IsCurrentUserInRole(ConstantRoles.Admin))
                {
                    _logger.LogWarning($"سعی در دسترسی غیرمجاز به ویرایش اطلاعات کاربر {pid}");
                    return BadRequest();
                }

                var user = await _userManager.FindByIdAsync(pid);

                if (user == null)
                    return NotFound();

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.IsEmailPublic = model.IsEmailPublic;
                user.TwoFactorEnabled = model.TwoFactorEnabled;
                user.Location = model.Location;

                UpdateUserBirthDate(model, user);

                if (!await UpdateUserName(model, user))
                    return model;

                if (!await UpdateUserAvatarImage(model, user))
                    return model;

                if (!await UpdateUserEmail(model, user))
                    return model;

                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    if (!model.IsAdminEdit)
                    {
                        // reflect the changes in the current user's Identity cookie
                        await _signInManager.RefreshSignInAsync(user);
                    }

                    await _emailSender.SendEmailAsync(
                           user.Email,
                           "اطلاع رسانی به روز رسانی مشخصات کاربری",
                           "~/Views/EmailTemplates/_UserProfileUpdateNotification.cshtml",
                           new UserProfileUpdateNotificationViewModel
                           {
                               User = user,
                               EmailSignature = _siteOptions.Value.Smtp.FromName,
                               MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                           });

                    return model;
                }

                ModelState.AddModelError("", updateResult.DumpErrors(true));
            }

            return model;
        }

        /// <summary>
        /// For [Remote] validation
        /// </summary>
        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult<string>> ValidateUsername(string username, string email, string pid)
        {
            pid = _protectionProviderService.Decrypt(pid);
            if (string.IsNullOrWhiteSpace(pid))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(pid);
            user.UserName = username;
            user.Email = email;

            var result = await _userValidator.ValidateAsync((UserManager<User>)_userManager, user);
            return result.DumpErrors(true);
        }

        private static void UpdateUserBirthDate(UserProfileViewModel model, User user)
        {
            if (model.DateOfBirthYear.HasValue &&
                model.DateOfBirthMonth.HasValue &&
                model.DateOfBirthDay.HasValue)
            {
                var date =
                    $"{model.DateOfBirthYear.Value.ToString()}/{model.DateOfBirthMonth.Value:00}/{model.DateOfBirthDay.Value:00}";

                user.Birthday = date.ToGregorianDateTime(true);
            }
            else
            {
                user.Birthday = null;
            }
        }

        private async Task<ApiResult<UserProfileViewModel>> RenderForm(User user, bool isAdminEdit)
        {
            _usersPhotoService.SetUserDefaultPhoto(user);

            var userProfile = new UserProfileViewModel
            {
                IsAdminEdit = isAdminEdit,
                Email = user.Email,
                PhotoFileName = user.PhotoFileName,
                Location = user.Location,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Pid = _protectionProviderService.Encrypt(user.Id.ToString()),
                IsEmailPublic = user.IsEmailPublic,
                TwoFactorEnabled = user.TwoFactorEnabled,
                IsPasswordTooOld = await _usedPasswordsService.IsLastUserPasswordTooOldAsync(user.Id)
            };

            if (!user.Birthday.HasValue)
                return userProfile;

            var pDateParts = user.Birthday.Value.ToPersianYearMonthDay();
            userProfile.DateOfBirthYear = pDateParts.Year;
            userProfile.DateOfBirthMonth = pDateParts.Month;
            userProfile.DateOfBirthDay = pDateParts.Day;

            return userProfile;
        }

        private async Task<bool> UpdateUserAvatarImage(UserProfileViewModel model, User user)
        {
            _usersPhotoService.SetUserDefaultPhoto(user);

            var photoFile = model.Photo;

            if (!(photoFile?.Length > 0))
                return true;

            var imageOptions = _siteOptions.Value.UserAvatarImageOptions;

            if (!photoFile.IsValidImageFile(imageOptions.MaxWidth, imageOptions.MaxHeight))
            {
                ModelState.AddModelError("",
                    $"حداکثر اندازه تصویر قابل ارسال {imageOptions.MaxHeight} در {imageOptions.MaxWidth} پیکسل است");
                model.PhotoFileName = user.PhotoFileName;

                return false;
            }

            var uploadsRootFolder = _usersPhotoService.GetUsersAvatarsFolderPath();
            var photoFileName = $"{user.Id}{Path.GetExtension(photoFile.FileName)}";
            var filePath = Path.Combine(uploadsRootFolder, photoFileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photoFile.CopyToAsync(fileStream);
            }

            user.PhotoFileName = photoFileName;
            return true;
        }

        private async Task<bool> UpdateUserEmail(UserProfileViewModel model, User user)
        {
            if (user.Email == model.Email)
                return true;

            user.Email = model.Email;
            var userValidator =
                await _userValidator.ValidateAsync((UserManager<User>)_userManager, user);

            if (!userValidator.Succeeded)
            {
                ModelState.AddModelError("", userValidator.DumpErrors(true));

                return false;
            }

            user.EmailConfirmed = false;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailSender.SendEmailAsync(
                user.Email,
                "لطفا اکانت خود را تائید کنید",
                "~/Views/EmailTemplates/_RegisterEmailConfirmation.cshtml",
                new RegisterEmailConfirmationViewModel
                {
                    User = user,
                    EmailConfirmationToken = code,
                    EmailSignature = _siteOptions.Value.Smtp.FromName,
                    MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                });

            return true;
        }

        private async Task<bool> UpdateUserName(UserProfileViewModel model, User user)
        {
            if (user.UserName == model.UserName)
                return true;

            user.UserName = model.UserName;
            var userValidator =
                await _userValidator.ValidateAsync((UserManager<User>)_userManager, user);

            if (userValidator.Succeeded)
                return true;

            ModelState.AddModelError("", userValidator.DumpErrors(true));

            return false;
        }
    }
}