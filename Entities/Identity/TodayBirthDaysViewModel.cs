using System.Collections.Generic;

namespace Entities.Identity
{
    public class TodayBirthDaysViewModel
    {
        public List<User.User> Users { set; get; }

        public AgeStatViewModel AgeStat { set; get; }
    }
}