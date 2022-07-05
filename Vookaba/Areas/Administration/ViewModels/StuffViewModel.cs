using Vookaba.Services.DTO;
using System.Collections.Generic;

namespace Vookaba.Areas.Administration.ViewModels
{
    public class StuffViewModel
    {
        public IEnumerable<StaffDTO> Administrators { get; init; }

        public IEnumerable<StaffDTO> Moderators { get; init; }

        public IEnumerable<StaffDTO> Janitors { get; init; }
    }
}
