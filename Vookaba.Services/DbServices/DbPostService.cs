using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vookaba.Common.Exceptions;
using Vookaba.DAL.Database;
using Vookaba.DAL.Entities;
using Vookaba.DAL.Entities.Base;
using Vookaba.Services.DTO;
using System.Linq.Expressions;
using Vookaba.Services.Abstractions;
using Vookaba.DAL.MediaStorage;

namespace Vookaba.Services.DbServices
{
    public class DbPostService : IPostService
    {
        private readonly VookabaDbContext context;
        private readonly IAttachmentsStorage attachmentStorage;
        private readonly IMapper mapper;
        private readonly ILogger<DbPostService> logger;

        public DbPostService(VookabaDbContext context,
            IAttachmentsStorage attachments,
            IMapper mapper,
            ILogger<DbPostService> logger)
        {
            this.context = context;
            this.attachmentStorage = attachments;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<PostDto?> GetByNumberAsync(string board, int number)
        {
            var post = await context.Posts
                .Include(p => p.Attachments)
                .FirstOrDefaultAsync(p => p.Number == number && p.Thread.BoardKey == board);
            return mapper.Map<PostDto>(post);
        }

        public Task DeleteByIdAsync(int id)
        {
            return DeleteRangeAsync(context.Posts.Where(p => p.Id == id));
        }

        public async Task DeleteManyAsync(int id, Mode mode, SearchArea area)
        {
            ThrowHelper.ThrowIfEnumIsNotCorrect(mode);
            ThrowHelper.ThrowIfEnumIsNotCorrect(area);

            var badPost = await context.Posts
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.IP,
                    p.AuthorToken,
                    p.ThreadId,
                    Board = p.Thread.BoardKey
                })
                .FirstOrDefaultAsync();

            if(badPost == null)
            {
                throw new ArgumentException($"Post with id {id} does not exist.");
            }

            //Expressions is fun (until you get runtime error)
            Expression selector = null!;
            var postParameter = Expression.Parameter(typeof(Post), "p");

            // select by ip
            // p.IP == badPost.IP
            if (mode.HasFlag(Mode.IP))
            {
                selector = Expression.Equal(
                    Expression.Property(postParameter, nameof(Post.IP)),
                    Expression.Constant(badPost.IP));
            }

            // Add selection by token to existing expression or create new.
            // For example: p.IP == badPost.IP || p.AthorToken == badPost.AuthorToken
            if (mode.HasFlag(Mode.Token))
            {
                var tok = Expression.Equal(
                    Expression.Property(postParameter, nameof(Post.AuthorToken)),
                    Expression.Constant(badPost.AuthorToken));
                selector = selector == null ? tok : Expression.Or(selector, tok);
            }


            var posts = context.Posts
                .Where(Expression.Lambda<Func<Post, bool>>(selector, postParameter));

            //limit posts by board
            Expression<Func<Post, bool>> boardSelector = p => p.Thread.BoardKey == badPost.Board;
            //limit posts by thread
            Expression<Func<Post, bool>> threadSelector = p => p.ThreadId == badPost.ThreadId;

            posts = area switch
            {
                SearchArea.All => posts,
                SearchArea.Board => posts.Where(boardSelector),
                SearchArea.Thread => posts.Where(boardSelector).Where(threadSelector),
                _ => throw new ArgumentException("Bad enum.", nameof(area))
            };

            await DeleteRangeAsync(posts);
        }

        private async Task DeleteRangeAsync(IQueryable<Post> postsT)
        {
            var posts = await postsT
                .Select(p => new { p.Id, p.IsOP, p.ThreadId })
                .ToListAsync();

            var tids = posts.Where(p => p.IsOP)
                .Select(p => p.ThreadId)
                .ToList();

            var pids = posts.Where(p => !p.IsOP)
                .Select(p => p.Id)
                .ToList();

            //if we do not stop the deleted threads,
            //then a post with an attachment may be created
            //and the attachmen will not be marked for deletion
            var threads = await context.Threads
                .AsTracking()
                .Where(t => tids.Contains(t.Id))
                .ToListAsync();
            threads.ForEach(t => t.IsReadOnly = true);
            await context.SaveChangesAsync();

            //attachment in threads
            var attachments = await context.Threads
                .Where(t => tids.Contains(t.Id) && t.IsReadOnly)
                .SelectMany(t => t.Posts)
                .SelectMany(p => p.Attachments)
                .Select(a => a.Name)
                .ToListAsync();

            //attachments in posts
            attachments.AddRange(await context.Set<Attachment>()
                .Where(a => pids.Contains(a.PostId))
                .Select(a => a.Name)
                .ToListAsync());

            if (tids.Count > 0)
            {
                var param = new Npgsql.NpgsqlParameter
                {
                    NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer,
                    ParameterName = "@ids",
                    Value = tids
                };
                await context.Database.ExecuteSqlRawAsync("call delete_threads(@ids)", param);
            }
            if (pids.Count > 0)
            {
                var param = new Npgsql.NpgsqlParameter
                {
                    NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer,
                    ParameterName = "@ids",
                    Value = pids
                };
                await context.Database.ExecuteSqlRawAsync("call delete_posts(@ids)", param);
            }

            await DeleteAttachments(attachments);
        }

        private async Task DeleteAttachments(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                try
                {
                    await attachmentStorage.DeleteImageAsync(name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Can't delete attachment.");
                }
            }
        }
    }
}
