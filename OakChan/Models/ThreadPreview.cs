using OakChan.Models.DB.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;

namespace OakChan.Models
{
    public class ThreadPreview : IEnumerable<Post>
    {
        public int Id { get; set; }

        public string Board { get; set; }

        public int PostCount { get; set; }

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
