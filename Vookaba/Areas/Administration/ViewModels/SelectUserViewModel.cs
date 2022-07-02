using Vookaba.Identity;
using System.Collections.Generic;
using Vookaba.ViewModels.Common;

namespace Vookaba.Areas.Administration.ViewModels
{
    public class SelectUserViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }

        public PaginatorViewModel PagesInfo { get; set; }
    }
}
