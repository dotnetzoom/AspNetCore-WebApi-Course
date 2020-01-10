using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Services.Identity
{
    public class ConfirmEmailDataProtectionTokenProviderOptions : DataProtectionTokenProviderOptions
    { }

    /// <summary>
    /// How to override the default (1 day) TokenLifeSpan for the email confirmations.
    /// </summary>
    public class ConfirmEmailDataProtectorTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public ConfirmEmailDataProtectorTokenProvider(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<ConfirmEmailDataProtectionTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
            // NOTE: DataProtectionTokenProviderOptions.TokenLifespan is set to TimeSpan.FromDays(1)
            // which is low for the `ConfirmEmail` task.
        }
    }
}