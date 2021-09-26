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
using OakChan.DAL;
using Microsoft.Extensions.Logging;

namespace OakChan.Services.DbServices
{
    public class DbBoardService : IBoardService
    {
        private readonly OakDbContext context;
        private readonly IMapper mapper;
        private readonly IAttachmentsStorage storage;
        private readonly ThrowHelper throwHelper;
        private readonly ILogger<DbBoardService> logger;

        public DbBoardService(OakDbContext context,
                              IMapper mapper,
                              IAttachmentsStorage storage,
                              ThrowHelper throwHelper,
                              ILogger<DbBoardService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.storage = storage;
            this.throwHelper = throwHelper;
            this.logger = logger;
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

        public async Task CreateBoardAsync(BoardDto board)
        {
            throwHelper.ThrowIfNull(board, nameof(board));

            var boardEntity = mapper.Map<Board>(board);
            try
            {
                context.Boards.Add(boardEntity);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is Npgsql.PostgresException x && x.SqlState == "23505")
            {
                throw new ArgumentException($"Board with key '{board.Key}' already exists.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Can't create board {board.Key}");
                throw;
            }
        }

        public async Task UpdateBoardAsync(string key, BoardDto board)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(key, nameof(key));
            throwHelper.ThrowIfNull(board, nameof(board));

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                //update key
                if (board.Key != null && board.Key != key)
                {
                    var cnt = await context.Database.ExecuteSqlInterpolatedAsync($@"UPDATE ""Boards"" b SET ""Key"" = {board.Key} WHERE b.""Key"" = {key}");
                    key = board.Key;
                }

                //update other properties
                var boardEntity = context.Boards.Find(key);
                var x = context.Attach(boardEntity);
                mapper.Map(board, boardEntity);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Can't update board {key}");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteBoardAsync(string boardKey)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));

            var board = await context.Boards.FindAsync(boardKey);
            if (board == null)
            {
                throw new ArgumentException($"Board with key '{board.Key}' do not exist.");
            }

            var attachments = await context.Boards
                .Where(b => b.Key == boardKey)
                .SelectMany(b => b.Threads.SelectMany(t => t.Posts))
                .SelectMany(p => p.Attachments)
                .AsNoTracking()
                .ToListAsync();

            context.Boards.Remove(board);
            await context.SaveChangesAsync();

            foreach (var attachment in attachments)
            {
                try
                {
                    await storage.DeleteImageAsync(attachment.Name);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Can't delete attachment {attachment.Id}:{attachment.Name}", ex);
                }
            }
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
