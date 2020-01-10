namespace Entities.Identity.Emails
{
    public class RegisterEmailConfirmationViewModel : EmailsBase
    {
        public User.User User { set; get; }
        public string EmailConfirmationToken { set; get; }
    }
}
