using OakChan.Identity;
using System.Collections.Generic;

namespace OakChan.Areas.Administration.ViewModels
{
    public class StuffViewModel
    {
        public IEnumerable<ApplicationUser> Moderators { get; init; }

        public IEnumerable<ApplicationUser> Janitors { get; init; }
    }
}
