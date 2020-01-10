using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Services.Contracts.Identity;

namespace Services.Identity
{
    public class ApplicationUserStore :
        UserStore<User, Role, ApplicationDbContext, int, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>,
        IApplicationUserStore
    {
        private readonly IUnitOfWork _uow;
        private readonly IdentityErrorDescriber _describer;

        public ApplicationUserStore(
            IUnitOfWork uow,
            IdentityErrorDescriber describer)
            : base((ApplicationDbContext)uow, describer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(_uow));
            _describer = describer ?? throw new ArgumentNullException(nameof(_describer));
        }

        #region BaseClass

        protected override UserClaim CreateUserClaim(User user, Claim claim)
        {
            var userClaim = new UserClaim { UserId = user.Id };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        protected override UserLogin CreateUserLogin(User user, UserLoginInfo login)
        {
            return new UserLogin
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        protected override UserRole CreateUserRole(User user, Role role)
        {
            return new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
        }

        protected override UserToken CreateUserToken(User user, string loginProvider, string name, string value)
        {
            return new UserToken
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }

        Task IApplicationUserStore.AddUserTokenAsync(UserToken token)
        {
            return base.AddUserTokenAsync(token);
        }

        Task<Role> IApplicationUserStore.FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return base.FindRoleAsync(normalizedRoleName, cancellationToken);
        }

        Task<UserToken> IApplicationUserStore.FindTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return base.FindTokenAsync(user, loginProvider, name, cancellationToken);
        }

        public Task RemoveUserTokenAsync(UserToken token)
        {
            throw new NotImplementedException();
        }

        Task<User> IApplicationUserStore.FindUserAsync(int userId, CancellationToken cancellationToken)
        {
            return base.FindUserAsync(userId, cancellationToken);
        }

        Task<UserLogin> IApplicationUserStore.FindUserLoginAsync(int userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return base.FindUserLoginAsync(userId, loginProvider, providerKey, cancellationToken);
        }

        Task<UserLogin> IApplicationUserStore.FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return base.FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
        }

        public Task<UserToken> FindTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<UserRole> IApplicationUserStore.FindUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken)
        {
            return base.FindUserRoleAsync(userId, roleId, cancellationToken);
        }

        Task IApplicationUserStore.RemoveUserTokenAsync(UserToken token)
        {
            return base.RemoveUserTokenAsync(token);
        }

        #endregion

        #region CustomMethods

        // Add custom methods here

        #endregion
    }
}