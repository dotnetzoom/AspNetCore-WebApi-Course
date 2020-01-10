using System.Collections.Generic;
using Entities.User;
using cloudscribe.Web.Pagination;

namespace Entities.Identity
{
    public class PagedUsersListViewModel
    {
        public PagedUsersListViewModel()
        {
            Paging = new PaginationSettings();
        }

        public List<User.User> Users { get; set; }

        public List<Role> Roles { get; set; }

        public PaginationSettings Paging { get; set; }
    }
}
