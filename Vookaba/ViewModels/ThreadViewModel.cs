using System.Collections.Generic;

namespace Vookaba.ViewModels
{
    public class ThreadViewModel : ThreadViewModelBase
    {
        public PostViewModel OpPost { get; set; }

        public IEnumerable<PostViewModel> Replies { get; set; }
    }
}
