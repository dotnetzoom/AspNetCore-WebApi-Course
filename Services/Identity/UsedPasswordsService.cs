using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using System;
using Entities.AuditableEntity;
using Entities.Identity.Settings;
using Entities.User;
using Services.Contracts.Identity;

namespace Services.Identity
{
    public class UsedPasswordsService : IUsedPasswordsService
    {
        private readonly int _changePasswordReminderDays;
        private readonly int _notAllowedPreviouslyUsedPasswords;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserUsedPassword> _userUsedPasswords;

        public UsedPasswordsService(
            IUnitOfWork uow,
            IPasswordHasher<User> passwordHasher,
            IOptionsSnapshot<SiteSettings> configurationRoot)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(_uow));

            _userUsedPasswords = _uow.Set<UserUsedPassword>() ?? throw new ArgumentNullException(nameof(_userUsedPasswords));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(_passwordHasher));
            if (configurationRoot == null) throw new ArgumentNullException(nameof(configurationRoot));
            var configurationRootValue = configurationRoot.Value;
            if (configurationRootValue == null) throw new ArgumentNullException(nameof(configurationRootValue));
            _notAllowedPreviouslyUsedPasswords = configurationRootValue.NotAllowedPreviouslyUsedPasswords;
            _changePasswordReminderDays = configurationRootValue.ChangePasswordReminderDays;
        }

        public async Task AddToUsedPasswordsListAsync(User user)
        {
            await _userUsedPasswords.AddAsync(new UserUsedPassword
            {
                UserId = user.Id,
                HashedPassword = user.PasswordHash
            });
            await _uow.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastUserPasswordChangeDateAsync(int userId)
        {
            var lastPasswordHistory =
                await _userUsedPasswords//.AsNoTracking() --> removes shadow properties
                                        .OrderByDescending(userUsedPassword => userUsedPassword.Id)
                                        .FirstOrDefaultAsync(userUsedPassword => userUsedPassword.UserId == userId);
            if (lastPasswordHistory == null)
            {
                return null;
            }

            var createdDateValue = _uow.GetShadowPropertyValue(lastPasswordHistory, AuditableShadowProperties.CreatedDateTime);
            return createdDateValue == null ?
                      (DateTime?)null :
                      DateTime.SpecifyKind((DateTime)createdDateValue, DateTimeKind.Utc);
        }

        public async Task<bool> IsLastUserPasswordTooOldAsync(int userId)
        {
            var createdDateTime = await GetLastUserPasswordChangeDateAsync(userId);
            if (createdDateTime == null)
            {
                return false;
            }
            return createdDateTime.Value.AddDays(_changePasswordReminderDays) < DateTime.UtcNow;
        }

        /// <summary>
        /// This method will be used by CustomPasswordValidator automatically,
        /// every time a user wants to change his/her info.
        /// </summary>
        public async Task<bool> IsPreviouslyUsedPasswordAsync(User user, string newPassword)
        {
            if (user.Id == 0)
            {
                // A new user wants to register at our site
                return false;
            }

            var userId = user.Id;
            var usedPasswords = await _userUsedPasswords
                                .AsNoTracking()
                                .Where(userUsedPassword => userUsedPassword.UserId == userId)
                                .OrderByDescending(userUsedPassword => userUsedPassword.Id)
                                .Select(userUsedPassword => userUsedPassword.HashedPassword)
                                .Take(_notAllowedPreviouslyUsedPasswords)
                                .ToListAsync();
            return usedPasswords.Any(hashedPassword => _passwordHasher.VerifyHashedPassword(user, hashedPassword, newPassword) != PasswordVerificationResult.Failed);
        }
    }
}