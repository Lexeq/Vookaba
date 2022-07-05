using Vookaba.Services.DTO;
using System.Collections.Generic;
using Vookaba.ViewModels.Common;

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
