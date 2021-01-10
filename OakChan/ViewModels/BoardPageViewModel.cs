using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class BoardPageViewModel
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public ICollection<ThreadPreviewViewModel> Threads { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }
    }
}
