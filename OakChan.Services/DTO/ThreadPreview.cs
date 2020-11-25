using OakChan.DAL.Entities;
using OakChan.Models.DB.Entities;
using OakChan.Services.DTO;
using System.Collections;
using System.Collections.Generic;

namespace OakChan.Services.DTO
{
    public class ThreadPreview : IEnumerable<Post>
    {
        public int Id { get; set; }

        public string Board { get; set; }

        public Post OpPost { get; set; }

        public int TotalPostsCount { get; set; }

        public int PostsWithImageCount { get; set; }

        public IEnumerable<Post> RecentPosts { get; set; }

        public IEnumerator<Post> GetEnumerator()
        {
            yield return OpPost;
            foreach (var post in RecentPosts)
            {
                yield return post;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
