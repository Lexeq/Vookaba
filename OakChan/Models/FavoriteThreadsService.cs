using OakChan.Models.DB;
using System;
using System.Linq;

namespace OakChan.Models
{
    public class FavoriteThreadsService
    {
        private readonly OakDbContext context;

        public FavoriteThreadsService(OakDbContext context)
        {
            this.context = context;
        }

        public TopThredInfo[] GetMostPopularPerBoard(int limit)
        {
            if (limit < 1)
            {
                throw new ArgumentException($"{nameof(limit)} must be greater then 0.", nameof(limit));
            }
            var toptop = context.Boards
                .SelectMany(b => b.Threads.OrderByDescending(t => t.Posts.Count()).Take(1))
                .OrderByDescending(t=>t.Posts.Count())
                .Take(3)
                .Select(t => new
                {
                    Thread = t,
                    PostCount = t.Posts.Count,
                    OpPost = t.Posts.OrderBy(p => p.CreationTime).First(),
                    Image = t.Posts.Select(p => p.Image).First(i => i.Id == t.Posts.OrderBy(p => p.CreationTime).First().ImageId)
                })
                .ToArray();

            return toptop.Select(p =>
            {
                p.OpPost.Image = p.Image;
                return new TopThredInfo { OpPost = p.OpPost, ThreadId = p.Thread.Id, BoardId = p.Thread.BoardId };
            }).ToArray();
        }
    }
}
