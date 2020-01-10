namespace Entities.Identity.Emails
{
    public class ChangePasswordNotificationViewModel : EmailsBase
    {
        public User.User User { set; get; }
    }
}