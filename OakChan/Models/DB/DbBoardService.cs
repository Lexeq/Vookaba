using Microsoft.EntityFrameworkCore;
using OakChan.Models.DB;
using OakChan.Models.DB.Entities;
using OakChan.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB
{
    public class DbBoardService : IBoardService
    {
        private const int RecentPostsCount = 2;

        private readonly OakDbContext context;
        private readonly PostCreator postCreator;

        public DbBoardService(OakDbContext context, PostCreator postCreator)
        {
            this.context = context;
            this.postCreator = postCreator;
        }

        public async Task<Thread> CreateThreadAsync(string boardId, PostCreationData data)
        {
            var thread = new Thread { BoardId = boardId };

            var post = await postCreator.AddPostToThread(data, thread);
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return thread;
        }

        public async Task<BoardPreview> GetBoardPreviewAsync(string boardId, int page, int threadsPerPage)
        {
            var b = await context.Boards.AsNoTracking()
                .Where(b => b.Key == boardId)
                .Select(b => new { Board = b, ThreadsCount = b.Threads.Count() })
                .FirstOrDefaultAsync();

            if (b == null)
            {
                return null;
            }

            var queryResult = await context.Threads.AsNoTracking()
                .Where(t => t.BoardId == boardId)
                .OrderByDescending(t => t.Posts.Max(p => p.CreationTime))
                .Skip((page - 1) * threadsPerPage)
                .Take(threadsPerPage)
                .Select(t => new
                {
                    Thread = t,
                    PostsCount = t.Posts.Count(),
                    ImagesCount = t.Posts.Where(p => p.Image != null).Count(),
                    OpPost = t.Posts.OrderBy(p => p.CreationTime).First(),
                    OpPic = t.Posts.OrderBy(p => p.CreationTime).Select(p => p.Image).First(),
                    RecentPosts = t.Posts.OrderByDescending(p => p.CreationTime).Take(RecentPostsCount),
                    RecentPostsImages = t.Posts.OrderByDescending(p => p.CreationTime).Select(p => p.Image).Take(RecentPostsCount)
                })
                .ToArrayAsync();

            //projecting query result on model classes
            var threadsOnPage = queryResult.Select(a =>
            {
                //set images for posts
                a.OpPost.Image = a.OpPic;
                foreach (var zipped in a.RecentPosts.Zip(a.RecentPostsImages, (post, image) => new { post, image }))
                {
                    zipped.post.Image = zipped.image;
                }

                return new ThreadPreview
                {
                    Id = a.Thread.Id,
                    Board = a.Thread.BoardId,
                    OpPost = a.OpPost,
                    TotalPostsCount = a.PostsCount,
                    PostsWithImageCount = a.ImagesCount,
                    RecentPosts = a.RecentPosts
                        .Reverse()
                        .Skip(a.PostsCount > RecentPostsCount ? 0 : 1) //exclude op post from recent posts
                        .ToArray()
                };
            })
            .ToArray();


            return new BoardPreview
            {
                Key = b.Board.Key,
                Name = b.Board.Name,
                PageNumber = page,
                Threads = threadsOnPage,
                TotalThreadsCount = b.ThreadsCount
            };

        }

        public async Task<IEnumerable<Board>> GetBoardsAsync()
        {
            return await context.Boards.AsNoTracking().ToArrayAsync();
        }
    }
}
