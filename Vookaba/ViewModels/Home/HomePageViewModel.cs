using Vookaba.Services.DTO;
using System.Collections.Generic;
using Vookaba.ViewModels.Thread;

namespace Vookaba.ViewModels.Home
{
    public class HomePageViewModel
    {
        public IEnumerable<BoardDto> Boards { get; set; }

        public IEnumerable<ThreadPreviewViewModel> LastCreatedThreads { get; set; }

        public IEnumerable<ThreadPreviewViewModel> LastUpdatedThreads { get; set; }
    }
}
