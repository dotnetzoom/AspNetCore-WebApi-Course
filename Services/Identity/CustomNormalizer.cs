using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Common.PersianToolkit;
using DNTPersianUtils.Core;

namespace Services.Identity
{
    public class CustomNormalizer : ILookupNormalizer
    {
        public string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            email = email.Trim();
            email = FixGmailDots(email);
            email = email.ToUpperInvariant();
            return email;
        }

        public string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            name = name.Trim();
            name = name.ApplyCorrectYeKe()
                 .RemoveDiacritics1()
                 .CleanUnderLines()
                 .RemovePunctuation();
            name = name.Trim().Replace(" ", "");
            name = name.ToUpperInvariant();
            return name;
        }

        private static string FixGmailDots(string email)
        {
            email = email.ToLowerInvariant().Trim();
            var emailParts = email.Split('@');
            var name = emailParts[0].Replace(".", string.Empty);

            var plusIndex = name.IndexOf("+", StringComparison.OrdinalIgnoreCase);
            if (plusIndex != -1)
            {
                name = name.Substring(0, plusIndex);
            }

            var emailDomain = emailParts[1];
            emailDomain = emailDomain.Replace("googlemail.com", "gmail.com");

            string[] domainsAllowedDots =
            {
                "gmail.com",
                "facebook.com"
            };

            var isFromDomainsAllowedDots = domainsAllowedDots.Any(domain => emailDomain.Equals(domain));
            return !isFromDomainsAllowedDots ? email : $"{name}@{emailDomain}";
        }
    }
}