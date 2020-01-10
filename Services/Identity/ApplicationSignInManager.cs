using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System;
using Entities.User;
using Services.Contracts.Identity;

namespace Services.Identity
{
    public class ApplicationSignInManager :
        SignInManager<User>,
        IApplicationSignInManager
    {
        private readonly IApplicationUserManager _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserClaimsPrincipalFactory<User> _claimsFactory;
        private readonly IOptions<IdentityOptions> _optionsAccessor;
        private readonly ILogger<ApplicationSignInManager> _logger;
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly IUserConfirmation<User> _confirmation;

        public ApplicationSignInManager(
            IApplicationUserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<ApplicationSignInManager> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> confirmation)
            : base((UserManager<User>)userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(_userManager));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(_contextAccessor));
            _claimsFactory = claimsFactory ?? throw new ArgumentNullException(nameof(_claimsFactory));
            _optionsAccessor = optionsAccessor ?? throw new ArgumentNullException(nameof(_optionsAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            _schemes = schemes ?? throw new ArgumentNullException(nameof(_schemes));
            _confirmation = confirmation;
        }

        #region BaseClass

        Task<bool> IApplicationSignInManager.IsLockedOut(User user)
        {
            return base.IsLockedOut(user);
        }

        Task<SignInResult> IApplicationSignInManager.LockedOut(User user)
        {
            return base.LockedOut(user);
        }

        Task<SignInResult> IApplicationSignInManager.PreSignInCheck(User user)
        {
            return base.PreSignInCheck(user);
        }

        Task IApplicationSignInManager.ResetLockout(User user)
        {
            return base.ResetLockout(user);
        }

        Task<SignInResult> IApplicationSignInManager.SignInOrTwoFactorAsync(User user, bool isPersistent, string loginProvider, bool bypassTwoFactor)
        {
            return base.SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
        }

        #endregion

        #region CustomMethods

        public bool IsCurrentUserSignedIn()
        {
            return IsSignedIn(_contextAccessor.HttpContext.User);
        }

        public Task<User> ValidateCurrentUserSecurityStampAsync()
        {
            return ValidateSecurityStampAsync(_contextAccessor.HttpContext.User);
        }

        #endregion
    }
}
