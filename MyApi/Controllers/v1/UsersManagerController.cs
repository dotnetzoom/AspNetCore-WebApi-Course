using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Common.IdentityToolkit;
using Entities.Identity;
using Entities.User;
using Services.Contracts.Identity;
using Services.Identity;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [Authorize(Roles = ConstantRoles.Admin)]
    public class UsersManagerController : BaseController
    {
        private const int DefaultPageSize = 7;

        private readonly IApplicationRoleManager _roleManager;
        private readonly IApplicationUserManager _userManager;

        public UsersManagerController(
            IApplicationUserManager userManager,
            IApplicationRoleManager roleManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(_roleManager));
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ActivateUserEmailStat(int userId)
        {
            User thisUser = null;
            var result = await _userManager.UpdateUserAndSecurityStampAsync(
                userId, user =>
                {
                    user.EmailConfirmed = true;
                    thisUser = user;
                });

            if (!result.Succeeded)
                return BadRequest(result.DumpErrors(true));

            return await ReturnUserCardPartialView(thisUser);
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ChangeUserLockoutMode(int userId, bool activate)
        {
            User thisUser = null;

            var result = await _userManager.UpdateUserAndSecurityStampAsync(
                userId, user =>
                {
                    user.LockoutEnabled = activate;
                    thisUser = user;
                });

            if (!result.Succeeded)
                return BadRequest(result.DumpErrors(true));

            return await ReturnUserCardPartialView(thisUser);
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ChangeUserRoles(int userId, int[] roleIds)
        {
            User thisUser = null;

            var result = await _userManager.AddOrUpdateUserRolesAsync(
                userId, roleIds, user => thisUser = user);

            if (!result.Succeeded)
                return BadRequest(result.DumpErrors(true));

            return await ReturnUserCardPartialView(thisUser);
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ChangeUserStat(int userId, bool activate)
        {
            User thisUser = null;

            var result = await _userManager.UpdateUserAndSecurityStampAsync(
                userId, user =>
                        {
                            user.IsActive = activate;
                            thisUser = user;
                        });

            if (!result.Succeeded)
                return BadRequest(result.DumpErrors(true));

            return await ReturnUserCardPartialView(thisUser);
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> ChangeUserTwoFactorAuthenticationStat(int userId, bool activate)
        {
            User thisUser = null;

            var result = await _userManager.UpdateUserAndSecurityStampAsync(
                userId, user =>
                {
                    user.TwoFactorEnabled = activate;
                    thisUser = user;
                });

            if (!result.Succeeded)
                return BadRequest(result.DumpErrors(true));

            return await ReturnUserCardPartialView(thisUser);
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult> EndUserLockout(int userId)
        {
            User thisUser = null;

            var result = await _userManager.UpdateUserAndSecurityStampAsync(
                userId, user =>
                {
                    user.LockoutEnd = null;
                    thisUser = user;
                });

            if (!result.Succeeded)
                return BadRequest(result.DumpErrors(true));

            return await ReturnUserCardPartialView(thisUser);
        }

        public async Task<ApiResult<PagedUsersListViewModel>> Index(int? page = 1, string field = "Id", SortOrder order = SortOrder.Ascending)
        {
            var model = await _userManager.GetPagedUsersListAsync(
                page.Value - 1,
                DefaultPageSize,
                field,
                order,
                true);

            model.Paging.CurrentPage = page.Value;
            model.Paging.ItemsPerPage = DefaultPageSize;
            model.Paging.ShowFirstLast = true;

            return model;
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ApiResult<SearchUsersViewModel>> SearchUsers(SearchUsersViewModel model)
        {
            var pagedUsersList = await _userManager.GetPagedUsersListAsync(
                model,
                0);

            pagedUsersList.Paging.CurrentPage = 1;
            pagedUsersList.Paging.ItemsPerPage = model.MaxNumberOfRows;
            pagedUsersList.Paging.ShowFirstLast = true;

            model.PagedUsersList = pagedUsersList;

            return model;
        }

        private async Task<ApiResult<UserCardItemViewModel>> ReturnUserCardPartialView(User thisUser)
        {
            var roles = await _roleManager.GetAllCustomRolesAsync();

            return new UserCardItemViewModel
            {
                User = thisUser,
                ShowAdminParts = true,
                Roles = roles,
                ActiveTab = UserCardItemActiveTab.UserAdmin
            };
        }
    }
}