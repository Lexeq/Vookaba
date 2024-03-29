﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Vookaba.DAL.Database;
using Vookaba.Services.DTO;
using Vookaba.Common.Exceptions;
using Vookaba.DAL.Entities;
using Microsoft.Extensions.Logging;
using Vookaba.Services.Abstractions;
using Vookaba.DAL.MediaStorage;

namespace Vookaba.Services.DbServices
{
    public class DbBoardService : IBoardService
    {
        private readonly VookabaDbContext context;
        private readonly IMapper mapper;
        private readonly IAttachmentsStorage storage;
        private readonly ILogger<DbBoardService> logger;

        public DbBoardService(VookabaDbContext context,
                              IMapper mapper,
                              IAttachmentsStorage storage,
                              ILogger<DbBoardService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.storage = storage;
            this.logger = logger;
        }

        public async Task<BoardDto?> GetBoardAsync(string boardKey)
        {
            ThrowHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));

            var board = await context.Boards
                .AsNoTracking()
                .Where(b => EF.Functions.ILike(b.Key, boardKey))
                .FirstOrDefaultAsync();

            var dto = mapper.Map<BoardDto>(board);
            return dto;
        }


        public async Task<PartialList<ThreadPreviewDto>> GetThreadPreviewsAsync(string boardKey, int offset, int count)
        {
            if (offset < 0) throw new ArgumentException("Offset must not be less than 0.", nameof(offset));
            if (count <= 0) throw new ArgumentException("Count must be greater than 0.", nameof(count));

            var totalCount = context.Threads.Count(t => t.BoardKey == boardKey);
            List<Thread> threadsOnPage = null!;
            if (totalCount > 0)
            {

                threadsOnPage = await context.Threads
                    .Where(t => t.BoardKey == boardKey && t.Posts.Any())
                    .OrderByDescending(t => t.IsPinned)
                    .ThenByDescending(t => t.LastBump)
                    .Skip(offset)
                    .Take(count)
                    .AsNoTracking()
                    .ToListAsync();

                var posts = (await context.Posts
                    .FromSqlInterpolated($"select * from posts_on_board_page({threadsOnPage.Select(t => t.Id).ToList()}, {Common.ApplicationConstants.BoardConstants.RecentRepliesShow})")
                    .Include(p => p.Attachments)
                    .AsNoTracking()
                    .ToListAsync())
                    .GroupBy(p => p.ThreadId)
                    .ToDictionary(p => p.Key, p => p.ToList());

                threadsOnPage.ForEach(t => t.Posts = posts[t.Id]);
            }
            var result = new PartialList<ThreadPreviewDto>(
                totalCount,
                mapper.Map<List<ThreadPreviewDto>>(threadsOnPage));
            return result;
        }

        public async Task<IEnumerable<BoardDto>> GetBoardsAsync(bool showAll)
        {
            return await mapper.ProjectTo<BoardDto>(context.Boards)
                .Where(b => showAll || !(b.IsHidden || b.IsDisabled))
                .OrderBy(b => b.Key)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreateBoardAsync(BoardDto board)
        {
            ThrowHelper.ThrowIfNull(board, nameof(board));

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
            ThrowHelper.ThrowIfNullOrWhiteSpace(key, nameof(key));
            ThrowHelper.ThrowIfNull(board, nameof(board));

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                //update key
                if (board.Key != null && board.Key != key)
                {
                    await context.Database.ExecuteSqlInterpolatedAsync($@"UPDATE ""Boards"" b SET ""Key"" = {board.Key} WHERE b.""Key"" = {key}");
                    key = board.Key;
                }

                //update other properties
                var boardEntity = context.Boards.Find(key);
                if (boardEntity == null)
                {
                    throw new ArgumentException("Board does not exist.");
                }
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
            ThrowHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));

            var board = await context.Boards.FindAsync(boardKey);
            if (board == null)
            {
                throw new ArgumentException($"Board with key '{boardKey}' does not exist.");
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
    }
}
