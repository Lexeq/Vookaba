using OakChan.Services.DTO;
using OakChan.Utils;
using OakChan.ViewModels;
using System.Collections.Generic;

namespace OakChan.Areas.Administration.ViewModels
{
    public class UserDetailsViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserRole { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public IList<CheckableItem<string>> Boards { get; set; }
    }
}
