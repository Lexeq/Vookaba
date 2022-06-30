using Vookaba.Services.DTO;
using System.Collections.Generic;

namespace Vookaba.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<BoardDto> Boards { get; set; }

        public IEnumerable<ThreadPreviewViewModel> LastCreatedThreads { get; set; }

        public IEnumerable<ThreadPreviewViewModel> LastUpdatedThreads { get; set; }
    }
}
