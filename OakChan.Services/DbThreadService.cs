using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OakChan.Common.Exceptions;
using OakChan.DAL.Database;
using OakChan.Services.DTO;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public class DbThreadService : IThreadService
    {
        private readonly OakDbContext context;
        private readonly IMapper mapper;
        private readonly IPostService posts;
        private readonly ThrowHelper throwHelper;

        public DbThreadService(OakDbContext context, IMapper mapper, IPostService posts, ThrowHelper throwHelper)
        {
            this.context = context;
            this.mapper = mapper;
            this.posts = posts;
            this.throwHelper = throwHelper;
        }

        public async Task<PostDto> AddPostToThreadAsync(string boardId, int threadId, PostCreationDto data)
        {
            throwHelper.ThrowIfNull(boardId, nameof(boardId));
            throwHelper.ThrowIfNull(data, nameof(data));

            var thread = await context.Threads.AsNoTracking().FirstOrDefaultAsync(t => t.BoardId == boardId && t.Id == threadId);

            if (thread == null)
            {
                throw new EntityNotFoundException($"Thread '{boardId}' does not exist.");
            }

            return await posts.CreatePost(new ThreadDto { ThreadId = threadId }, data);
        }

        public async Task<ThreadDto> GetThreadAsync(string boardId, int threadId)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardId, nameof(boardId));

            var thread = await context.Threads.AsNoTracking()
                .Where(t => t.BoardId == boardId && t.Id == threadId)
                .Include(t => t.Posts)
                .ThenInclude(p => p.Image)
                .FirstOrDefaultAsync();

            return mapper.Map<ThreadDto>(thread);
        }
    }
}
