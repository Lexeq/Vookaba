using System.Collections.Generic;

namespace OakChan.Services.DTO
{
    public class ThreadPreviewDto
    {
        public int ThreadId { get; set; }

        public string Board { get; set; }

        public PostDto OpPost { get; set; }

        public int TotalPostsCount { get; set; }

        public int PostsWithImageCount { get; set; }

        public IEnumerable<PostDto> RecentPosts { get; set; }
    }
}
