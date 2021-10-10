using OakChan.Services.DTO;
using System.Collections.Generic;

namespace OakChan.Areas.Administration.ViewModels
{
    public class StuffViewModel
    {
        public IEnumerable<StaffDTO> Moderators { get; init; }

        public IEnumerable<StaffDTO> Janitors { get; init; }
    }
}
