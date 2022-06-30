using System.Collections.Generic;
using System.Linq;

namespace Vookaba.ViewModels
{
    public class ThreadPreviewViewModel : ThreadViewModelBase
    {
        public int PostsCount { get; set; }

        public int PostsWithImageCount { get; set; }

        public PostViewModel OpPost { get; set; }

        public IEnumerable<PostViewModel> RecentPosts { get; set; }

        public int PostsOmmited => PostsCount - (1 + RecentPosts.Count());

        public int ImagesOmmited => PostsWithImageCount - (OpPost.HasImage ? 1 : 0 + RecentPosts.Count(p => p.HasImage));

    }

}
