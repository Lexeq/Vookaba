using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.Services.DTO;
using OakChan.Services.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OakChan.Common.Exceptions;

namespace OakChan.Services
{
    public class DbBoardService : IBoardService
    {
        private const int RecentPostsCount = 2;

        private readonly OakDbContext context;
        private readonly IMapper mapper;
        private readonly IPostService posts;
        private readonly ThrowHelper throwHelper;

        public DbBoardService(OakDbContext context, IMapper mapper, IPostService posts, ThrowHelper throwHelper)
        {
            this.context = context;
            this.mapper = mapper;
            this.posts = posts;
            this.throwHelper = throwHelper;
        }

        public async Task<ThreadDto> CreateThreadAsync(string boardId, ThreadCreationDto threadDto)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardId, nameof(boardId));
            throwHelper.ThrowIfNull(threadDto, nameof(threadDto));

            if (await context.Boards.FindAsync(boardId) == null)
            {
                throw new KeyNotFoundException($"Board '{boardId}' does not exist.");
            }

            var thread = new ThreadDto { Subject = threadDto.Subject, BoardId = boardId };

            var post = await posts.CreatePost(thread, threadDto.OpPost);

            return new ThreadDto
            {
                BoardId = boardId,
                Subject = threadDto.Subject,
                ThreadId = post.ThreadId,
                OpPost = post,
                Replies = Enumerable.Empty<PostDto>()
            };
        }

        public async Task<BoardInfoDto> GetBoardInfoAsync(string boardId)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardId, nameof(boardId));

            var result = await mapper.ProjectTo<BoardInfoDto>(context.Boards.AsNoTracking())
                .Where(b => b.Key == boardId)
                .FirstOrDefaultAsync();

            return result ?? throw new KeyNotFoundException($"Board '{boardId}' does not exist.");
        }

        public async Task<BoardPageDto> GetBoardPageAsync(string boardId, int page, int pageSize)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardId, nameof(boardId));
            if (page <= 0) throw new ArgumentException("Page number must be greater than 0.", nameof(page));
            if (pageSize <= 0) throw new ArgumentException("Page number must be greater than 0.", nameof(pageSize));

            var offset = (page - 1) * pageSize;

            var pageThreads = await GetThreadPreviews(boardId, offset, pageSize);

            return new BoardPageDto
            {
                BoardId = boardId,
                PageNumber = page,
                Threads = pageThreads,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<BoardInfoDto>> GetBoardsAsync()
        {
            return await mapper.ProjectTo<BoardInfoDto>(context.Boards)
                .OrderBy(b => b.Key)
                .ToArrayAsync();
        }

        private async Task<IEnumerable<ThreadPreviewDto>> GetThreadPreviews(string board, int offset, int count)
        {
            var threadsQuery = context.Threads.AsNoTracking()
               .Where(t => t.BoardId == board)
               .OrderByDescending(t => t.Posts.Max(p => p.CreationTime))
               .Skip(offset)
               .Take(count);

            var resultQuery = threadsQuery
                .Select(t => new ThreadPreviewQueryResult
                {
                    ThreadId = t.Id,
                    BoardId = t.BoardId,
                    Subject = t.Subject,
                    PostsCount = t.Posts.Count(),
                    ImagesCount = t.Posts.Where(p => p.Image != null).Count(),
                    OpPost = t.Posts.AsQueryable()
                                        .Include(p => p.Image)
                                        .OrderBy(p => p.CreationTime)
                                        .First(),
                    RecentPosts = t.Posts.AsQueryable()
                                        .Include(p => p.Image)
                                        .OrderBy(p => p.CreationTime)
                                        .Skip(1)
                                        .OrderByDescending(p => p.CreationTime)
                                        .Take(RecentPostsCount)
                                        .OrderBy(p => p.CreationTime)
                                        .ToList()
                });

            var result = mapper.Map<List<ThreadPreviewDto>>(await resultQuery.ToListAsync());
            return result;
        }
    }
}
