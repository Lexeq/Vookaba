using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public class DbThreadService : IThreadService
    {
        private readonly OakDbContext context;
        private readonly IMapper mapper;
        private readonly PostCreator postCreator;

        public DbThreadService(OakDbContext context, IMapper mapper, PostCreator postCreator)
        {
            this.context = context;
            this.mapper = mapper;;
            this.postCreator = postCreator;
        }

        public async Task<Post> CreatePostAsync(string board, int thread, PostCreationData data)
        {
            var t = await context.Threads.FirstOrDefaultAsync(t => t.Id == thread && t.BoardId == board);
            if (t == null)
            {
                throw new Exception();
            }

            var post = await postCreator.AddPostToThread(data, t);
            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return post;
        }

        public async Task<ThreadDto> GetThreadAsync(string boardId, int threadId)
        {
            var thread = await context.Threads.AsNoTracking()
                .Where(t => t.BoardId == boardId && t.Id == threadId)
                .Include(t => t.Posts)
                .ThenInclude(p => p.Image)
                .FirstOrDefaultAsync();

            return mapper.Map<ThreadDto>(thread);
        }
    }
}
