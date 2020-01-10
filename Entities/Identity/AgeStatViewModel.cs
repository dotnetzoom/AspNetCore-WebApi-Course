namespace Entities.Identity
{
    public class AgeStatViewModel
    {
        public const char RleChar = (char)0x202B;

        public int UsersCount { set; get; }
        public int AverageAge { set; get; }
        public User.User MaxAgeUser { set; get; }
        public User.User MinAgeUser { set; get; }
    }
}