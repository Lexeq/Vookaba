using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class ThreadPreviewViewModel
    {
        public int ThreadId { get; set; }

        public int PostsCount { get; set; }

        public int PostsWithImageCount { get; set; }

        public PostViewModel OpPost { get; set; }

        public IEnumerable<PostViewModel> RecentPosts { get; set; }
    }

}
