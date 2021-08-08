using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Services.DbServices
{

    public class TopThreadsService : ITopThreadsService
    {
        private readonly OakDbContext context;
        private readonly IMapper mapper;

        public TopThreadsService(OakDbContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper;
        }

        public Task<IEnumerable<ThreadPreviewDto>> GetTopThreadsByLastPostAsync(int limit)
        {
            var threads = context.Threads
               .FromSqlRaw("SELECT * FROM public.\"last_bumped_threads_per_board\"")
               .Where(t => !(t.Board.IsDisabled || t.Board.IsHidden))
               .OrderByDescending(t1 => t1.LastBump)
               .Take(limit);

            return LoadThreadPreviews(threads);
        }

        public Task<IEnumerable<ThreadPreviewDto>> GetTopThreadsByCreationTimeAsync(int limit)
        {
            var threads = context.Threads
                .FromSqlRaw("SELECT * FROM public.\"last_created_threads_per_board\"")
                .Where(t => !(t.Board.IsDisabled || t.Board.IsHidden))
                .OrderByDescending(t1 => t1.Created)
                .Take(limit);

            return LoadThreadPreviews(threads);
        }

        private async Task<IEnumerable<ThreadPreviewDto>> LoadThreadPreviews(IQueryable<Thread> queryableThreads)
        {
            var threads = await queryableThreads.AsNoTracking().ToListAsync();

            var posts = await context.Posts
                .Include(p => p.Attachments)
                .Where(p => threads.Select(t => t.Id).Contains(p.ThreadId) && p.IsOP)
                .ToListAsync();

            return mapper.Map<IEnumerable<ThreadPreviewDto>>(threads.Select(t =>
                {
                    t.Posts = new[] { posts.First(p => p.ThreadId == t.Id) };
                    return t;
                }));
        }
    }
}
