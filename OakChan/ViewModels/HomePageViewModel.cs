using OakChan.Services.DTO;
using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<BoardInfoDto> Boards { get; set; }

        public IEnumerable<ThreadPreviewViewModel> LastCreatedThreads { get; set; }

        public IEnumerable<ThreadPreviewViewModel> LastUpdatedThreads { get; set; }
    }
}
