using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using OakChan.Models.DB;
using OakChan.Models.DB.Entities;
using OakChan.Models.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB
{
    public class DbThreadService : IThreadService
    {
        private readonly OakDbContext context;
        private readonly PostCreator postCreator;

        public DbThreadService(OakDbContext context, PostCreator postCreator)
        {
            this.context = context;
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

        public async Task<Thread> GetThreadAsync(string board, int thread)
        {
            var t = await context.Threads.AsNoTracking()
                .Where(t => t.BoardId == board && t.Id == thread)
                .Include(t => t.Posts)
                .ThenInclude(p => p.Image)
                .FirstOrDefaultAsync();

            return t;
        }
    }
}
