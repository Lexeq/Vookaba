using OakChan.DAL.Database;
using OakChan.Services.DTO;
using System;
using System.Linq;

namespace OakChan.Services
{
    public class FavoriteThreadsService
    {
        private readonly OakDbContext context;

        public FavoriteThreadsService(OakDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TopThredInfo[] GetMostPopularThreadOnBoard(int boardsLimit)
        {
            if (boardsLimit < 1)
            {
                throw new ArgumentException($"{nameof(boardsLimit)} must be greater then 0.", nameof(boardsLimit));
            }
            var toptop = context.Boards
                .SelectMany(b => b.Threads.OrderByDescending(t => t.Posts.Count()).Take(1))
                .OrderByDescending(t => t.Posts.Count())
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
