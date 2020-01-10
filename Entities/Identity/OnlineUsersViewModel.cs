using System.Collections.Generic;

namespace Entities.Identity
{
    public class OnlineUsersViewModel
    {
        public List<User.User> Users { set; get; }
        public int NumbersToTake { set; get; }
        public int MinutesToTake { set; get; }
        public bool ShowMoreItemsLink { set; get; }
    }
}