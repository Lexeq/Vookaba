using OakChan.DAL.Entities;
using System.Collections.Generic;

namespace OakChan.Services.Internal
{
    internal class ThreadPreviewQueryResult
    {
        public int ThreadId { get; set; }
        public string Subject { get; set; }
        public int PostsCount { get; set; }
        public int ImagesCount { get; set; }
        public Post OpPost { get; set; }
        public ICollection<Post> RecentPosts { get; set; }
        public string BoardId { get; internal set; }
    }
}
