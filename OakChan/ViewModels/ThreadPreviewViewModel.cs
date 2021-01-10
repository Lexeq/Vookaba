using System.Collections.Generic;
using System.Linq;

namespace OakChan.ViewModels
{
    public class ThreadPreviewViewModel
    {
        public int ThreadId { get; set; }

        public int PostsCount { get; set; }

        public string Subject { get; set; }

        public int PostsWithImageCount { get; set; }

        public OpPostViewModel OpPost { get; set; }

        public IEnumerable<PostViewModel> RecentPosts { get; set; }

        public int PostsOmmited => PostsCount - (1 + RecentPosts.Count());

        public int ImagesOmmited => PostsWithImageCount - (OpPost.HasImage ? 1 : 0 + RecentPosts.Count(p => p.HasImage));

    }

}
