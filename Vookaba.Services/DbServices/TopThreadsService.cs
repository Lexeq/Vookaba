using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Vookaba.DAL.Database;
using Vookaba.DAL.Entities;
using Vookaba.Services.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Vookaba.Services.DbServices
{

    public class TopThreadsService : ITopThreadsService
    {
        private readonly VookabaDbContext context;
        private readonly IMapper mapper;

        public TopThreadsService(VookabaDbContext context, IMapper mapper)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mapper = mapper;
        }

        public Task<IEnumerable<ThreadPreviewDto>> GetLastRepliedThreadsAsync(int limit)
        {
            var threads = context.Threads
               .FromSqlRaw("SELECT * FROM public.\"last_bumped_threads_per_board\"")
               .OrderByDescending(t1 => t1.LastBump)
               .Take(limit);

            return LoadThreadPreviews(threads);
        }

        public Task<IEnumerable<ThreadPreviewDto>> GetLastCreatedThreadsAsync(int limit)
        {
            var threads = context.Threads
                .FromSqlRaw("SELECT * FROM public.\"last_created_threads_per_board\"")
                .OrderByDescending(t1 => t1.Created)
                .Take(limit);

            return LoadThreadPreviews(threads);
        }

        private async Task<IEnumerable<ThreadPreviewDto>> LoadThreadPreviews(IQueryable<Thread> queryableThreads)
        {
            var threads = await queryableThreads
                .Include(t => t.Posts.Where(x => x.IsOP).Take(1))
                .ThenInclude(p => p.Attachments)
                .ToArrayAsync();

            return mapper.Map<IEnumerable<ThreadPreviewDto>>(threads);
        }
    }
}
