using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OakChan.Common.Exceptions;
using OakChan.DAL.Entities;

namespace OakChan.Services.DbServices
{
    public class DbBoardService : IBoardService
    {
        private readonly OakDbContext context;
        private readonly IMapper mapper;
        private readonly ThrowHelper throwHelper;

        public DbBoardService(OakDbContext context, IMapper mapper, ThrowHelper throwHelper)
        {
            this.context = context;
            this.mapper = mapper;
            this.throwHelper = throwHelper;
        }

        public async Task<BoardInfoDto> GetBoardInfoAsync(string boardKey)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));

            var board = await context.Boards
                .AsNoTracking()
                .Where(b => EF.Functions.ILike(b.Key, boardKey))
                .Select(b => new { Board = b, Count = b.Threads.Count() })
                .FirstOrDefaultAsync();

            if (board == null)
            {
                return null;
            }

            var dto = mapper.Map<BoardInfoDto>(board.Board);
            dto.ThreadsCount = board.Count;
            return dto;
        }

        public async Task<IEnumerable<ThreadPreviewDto>> GetThreadPreviewsAsync(string boardKey, int offset, int count, int recentPostsCount)
        {
            if (offset < 0) throw new ArgumentException("Offset must not be less than 0.", nameof(offset));
            if (count <= 0) throw new ArgumentException("Count must be greater than 0.", nameof(count));

            var threadsOnPage = await context.Threads
                .Where(t => t.BoardKey == boardKey && t.Posts.Any())
                .OrderByDescending(t => t.IsPinned)
                .ThenByDescending(t => t.LastBump)
                .Skip(offset)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();

            if (!threadsOnPage.Any())
            {
                return Enumerable.Empty<ThreadPreviewDto>();
            }

            var threadIds = threadsOnPage.Select(t => t.Id);

            var posts = (await GetFirstAndLastPostsQuery(threadIds, recentPostsCount)
                .Include(p => p.Attachments)
                .AsNoTracking()
                .ToListAsync())
                .GroupBy(p => p.ThreadId)
                .ToDictionary(g => g.Key, g => g.OrderBy(p => p.Number).ToList());

            return threadsOnPage.Select(t =>
            {
                var dto = mapper.Map<ThreadPreviewDto>(t);
                dto.Posts = mapper.Map<IEnumerable<PostDto>>(posts[t.Id]);
                return dto;
            });
        }

        public async Task<IEnumerable<BoardInfoDto>> GetBoardsAsync(bool showAll)
        {
            return await mapper.ProjectTo<BoardInfoDto>(context.Boards)
                .Where(b => showAll || !(b.IsHidden || b.IsDisabled))
                .OrderBy(b => b.Key)
                .AsNoTracking()
                .ToListAsync();
        }

        private IQueryable<Post> GetFirstAndLastPostsQuery(IEnumerable<int> threadIds, int recentPostsCount)
        {
            var idsString = string.Join(", ", threadIds);
            var queryString =
@$"
SELECT op.*
FROM ""Threads"" t,
LATERAL(
    SELECT *
    FROM ""Posts"" p0
    WHERE p0.""IsOP"" and p0.""ThreadId"" = t.""Id""
    UNION(
        SELECT *
        FROM ""Posts"" p1
        WHERE p1.""ThreadId"" = t.""Id""
        ORDER BY p1.""Number"" DESC
        LIMIT {recentPostsCount})
    ) AS op
WHERE t.""Id"" IN({idsString})
";
            return context.Posts.FromSqlRaw(queryString);
        }
    }
}
