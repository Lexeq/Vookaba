using System.Collections.Generic;

namespace Vookaba.ViewModels
{
    public class BoardPageViewModel
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public bool IsReadOnly { get; set; }

        public IEnumerable<ThreadPreviewViewModel> Threads { get; set; }

        public PaginatorViewModel PagesInfo { get; set; }
    }
}
