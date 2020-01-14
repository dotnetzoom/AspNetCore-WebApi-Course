using Entities.Common;

namespace Entities.Identity
{
    public class AppDataProtectionKey :BaseEntity
    {
        public string FriendlyName { get; set; }
        public string XmlData { get; set; }
    }
}