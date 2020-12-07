using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public class DbBoardService : IBoardService
    {
        private const int RecentPostsCount = 2;

        private readonly OakDbContext context;
        private readonly IMapper mapper;
        private readonly PostCreator postCreator;

        public DbBoardService(OakDbContext context, IMapper mapper, PostCreator postCreator)
        {
            this.context = context;
            this.mapper = mapper;
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

        public async Task<BoardInfoDto> GetBoardAsync(string board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }
            var result = await context.Boards.AsNoTracking()
                .Where(b => b.Key == board)
                .Select(b => new BoardInfoDto
                {
                    Key = b.Key,
                    Name = b.Name,
                    ThreadsCount = b.Threads.Count()
                })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<BoardPageDto> GetBoardPageAsync(string board, int page, int pageSize)
        {
            var offset = (page - 1) * pageSize;

            var queryResult = await context.Threads.AsNoTracking()
                .Where(t => t.BoardId == board)
                .OrderByDescending(t => t.Posts.Max(p => p.CreationTime))
                .Skip(offset)
                .Take(pageSize)
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

                return new ThreadPreviewDto
                {
                    ThreadId = a.Thread.Id,
                    Board = a.Thread.BoardId,
                    OpPost = mapper.Map<PostDto>(a.OpPost),
                    TotalPostsCount = a.PostsCount,
                    PostsWithImageCount = a.ImagesCount,
                    RecentPosts = mapper.Map<PostDto[]>(
                        a.RecentPosts
                         .Reverse()
                         .Skip(a.PostsCount > RecentPostsCount ? 0 : 1) //exclude op post from recent posts
                        )
                };
            })
            .ToArray();


            return new BoardPageDto
            {
                BoardId = board,
                PageNumber = page,
                Threads = threadsOnPage
            };

        }

        public async Task<IEnumerable<Board>> GetBoardsAsync()
        {
            return await context.Boards.AsNoTracking().ToArrayAsync();
        }
    }
}
