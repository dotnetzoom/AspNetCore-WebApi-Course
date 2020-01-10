namespace Entities.Identity.Emails
{
    public class UserProfileUpdateNotificationViewModel : EmailsBase
    {
        public User.User User { set; get; }
    }
}