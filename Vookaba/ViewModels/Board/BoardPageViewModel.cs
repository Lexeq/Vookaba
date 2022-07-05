using System.Collections.Generic;
using Vookaba.ViewModels.Common;
using Vookaba.ViewModels.Thread;

namespace Vookaba.ViewModels.Board
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
