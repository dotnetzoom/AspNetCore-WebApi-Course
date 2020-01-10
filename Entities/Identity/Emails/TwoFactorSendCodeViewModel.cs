namespace Entities.Identity.Emails
{
    public class TwoFactorSendCodeViewModel : EmailsBase
    {
        public string Token { set; get; }
    }
}