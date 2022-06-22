using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class ThreadViewModel : ThreadViewModelBase
    {
        public PostViewModel OpPost { get; set; }

        public IEnumerable<PostViewModel> Replies { get; set; }
    }
}
