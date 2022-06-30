using Vookaba.Services.DTO;
using Vookaba.Utils;
using Vookaba.ViewModels;
using System.Collections.Generic;

namespace Vookaba.Areas.Administration.ViewModels
{
    public class UserDetailsViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserRole { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public IList<CheckableItem<string>> Boards { get; set; }

        public IEnumerable<ModLogDto> Logs { get; set; }

        public bool IsEditable { get; set; }

        public PaginatorViewModel PageInfo { get; set; }
    }
}
