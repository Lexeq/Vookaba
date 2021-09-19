using OakChan.Services.DTO;
using System.Collections.Generic;

namespace OakChan.Areas.Administration.ViewModels
{
    public class AdminDashboardViemModel
    {
        public IEnumerable<BoardInfoDto> Boards { get; set; }

        public StuffViewModel Staff { get; set; }
    }
}
