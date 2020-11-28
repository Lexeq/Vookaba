using OakChan.Models;
using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class BoardViewModel
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public IEnumerable<ThreadPreviewViewModel> ThreadsOnPage { get; set; }

        public OpPostFormViewModel OpPost { get; set; }
        
        public int PageNumber { get; set; }

        public int TotalPages { get; set; }
        public int TotalThreadsCount { get; internal set; }
    }
}
