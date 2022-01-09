using OakChan.Identity;
using OakChan.ViewModels;
using System.Collections.Generic;

namespace OakChan.Areas.Administration.ViewModels
{
    public class SelectUserViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }

        public PaginatorViewModel PagesInfo { get; set; }
    }
}
