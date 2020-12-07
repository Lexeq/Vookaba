using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class BoardPageViewModel
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public ICollection<ThreadPreviewViewModel> ThreadsOnPage { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public OpPostFormViewModel OpPost { get; set; }
    }
}
