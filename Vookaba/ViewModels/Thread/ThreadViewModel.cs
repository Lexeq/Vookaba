using System.Collections.Generic;
using Vookaba.ViewModels.Post;

namespace Vookaba.ViewModels.Thread
{
    public class ThreadViewModel : ThreadViewModelBase
    {
        public PostViewModel OpPost { get; set; }

        public IEnumerable<PostViewModel> Replies { get; set; }
    }
}
