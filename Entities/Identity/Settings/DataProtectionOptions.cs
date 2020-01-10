using System;

namespace Entities.Identity.Settings
{
    public class DataProtectionOptions
    {
        public TimeSpan DataProtectionKeyLifetime { get; set; }
        public string ApplicationName { get; set; }
    }
}