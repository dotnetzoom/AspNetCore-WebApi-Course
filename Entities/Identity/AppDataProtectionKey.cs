using Entities.Common;

namespace Entities.Identity
{
    public class AppDataProtectionKey: IEntity
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public string XmlData { get; set; }
    }
}