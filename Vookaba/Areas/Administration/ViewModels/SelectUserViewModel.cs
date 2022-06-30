using Vookaba.Identity;
using Vookaba.ViewModels;
using System.Collections.Generic;

namespace Vookaba.Areas.Administration.ViewModels
{
    public class SelectUserViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }

        public PaginatorViewModel PagesInfo { get; set; }
    }
}
