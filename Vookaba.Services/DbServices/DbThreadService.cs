﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Vookaba.Common.Exceptions;
using Vookaba.DAL.Database;
using Vookaba.DAL.Entities;
using Vookaba.DAL.Entities.Base;
using Vookaba.Services.DTO;
using System.Text;
using Vookaba.Services.Abstractions;
using Vookaba.DAL.MediaStorage;

namespace Vookaba.Services.DbServices
{
    public class DbThreadService : IThreadService
    {
        private static string GetThreadSubjectFromMesage(ThreadCreationDto threadCreation)
        {
            if (!string.IsNullOrEmpty(threadCreation.Subject))
            {
                return threadCreation.Subject;
            }
            var message = threadCreation.OpPost.Message;
            var subjMax = Common.ApplicationConstants.ThreadConstants.SubjectMaxLength;
            var overflowStr = "...";

            var builder = new StringBuilder(subjMax);
            for (int i = 0; i < message!.Length; i++) //OP post must contains message, so it's can't be null.
            {
                if (message[i] != '<')
                {
                    if (builder.Length == subjMax)
                    {
                        builder.Insert(subjMax - overflowStr.Length, overflowStr).Length = subjMax;
                        break;
                    }
                    builder.Append(message[i]);
                }
                else
                {
                    i = message.IndexOf('>', i + 1);
                }
            }
            return builder.ToString();
        }

        private readonly VookabaDbContext context;
        private readonly IAttachmentsStorage attachmentsStorage;
        private readonly IHashService hashService;
        private readonly IEnumerable<IPostProcessor> processors;
        private readonly IMapper mapper;

        public DbThreadService(VookabaDbContext context,
                               IAttachmentsStorage attachmentsStorage,
                               IHashService hashService,
                               IEnumerable<IPostProcessor> processors,
                               IMapper mapper)
        {
            this.context = context;
            this.attachmentsStorage = attachmentsStorage;
            this.hashService = hashService;
            this.processors = processors;
            this.mapper = mapper;
        }

        public async Task<ThreadDto?> GetThreadAsync(string boardKey, int threadId)
        {
            ThrowHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));

            return mapper.Map<ThreadDto>(
                await context.Threads.AsNoTracking()
                    .Where(t => t.BoardKey == boardKey && t.Id == threadId)
                    .Include(t => t.Posts.OrderBy(p => p.Number))
                    .ThenInclude(p => p.Attachments)
                    .FirstOrDefaultAsync());
        }

        public Task<ThreadInfoDto?> GetThreadInfoAsync(string boardKey, int threadId)
        {
            ThrowHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));

            return context.Threads
                .Where(t => t.BoardKey == boardKey && t.Id == threadId)
                .ProjectTo<ThreadInfoDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<ThreadDto> CreateThreadAsync(string boardKey, ThreadCreationDto threadDto)
        {
            ThrowHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));
            ThrowHelper.ThrowIfNull(threadDto, nameof(threadDto));

            var post = await CreatePostEntityAsync(threadDto.OpPost);
            var thread = new Thread { BoardKey = boardKey, Subject = GetThreadSubjectFromMesage(threadDto) };
            post.IsOP = true;
            post.Thread = thread;

            context.Posts.Add(post);

            await context.SaveChangesAsync();

            //TODO: Use automapper?

            return new ThreadDto
            {
                BoardKey = boardKey,
                ThreadId = thread.Id,
                Subject = threadDto.Subject,
                Posts = new[] { mapper.Map<PostDto>(post) }
            };
        }

        public async Task<PostDto> AddPostToThreadAsync(int threadId, PostCreationDto postData)
        {
            ThrowHelper.ThrowIfNull(postData, nameof(postData));

            var post = await CreatePostEntityAsync(postData);
            post.ThreadId = threadId;

            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return mapper.Map<PostDto>(post);
        }

        private async Task<Post> CreatePostEntityAsync(PostCreationDto postDto)
        {
            ThrowHelper.ThrowIfNull(postDto, nameof(postDto));

            foreach (var proc in processors)
            {
                await proc.ProcessAsync(postDto);
            }

            var post = mapper.Map<Post>(postDto);
            if (postDto.Attachments != null && postDto.Attachments.Any())
            {
                post.Attachments = await CreateImageEntityAsync(postDto.Attachments);
            }
            return post;
        }

        //TODO: Support different attachment types
        //TODO: Extract attachment creation to a separate class? 
        private async Task<List<Attachment>> CreateImageEntityAsync(IFormFileCollection files)
        {
            var attachments = new List<Attachment>(files.Count);
            foreach (var file in files)
            {
                attachments.Add(await CreateImageAttachment(file));
            }
            return attachments;
        }

        private string GenerateFileName()
            => Guid.NewGuid().ToString("N");

        private string HashBytesToString(byte[] hash)
            => BitConverter.ToString(hash).Replace("-", string.Empty);

        private async Task<Image> CreateImageAttachment(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
            string imageFullName = $"{GenerateFileName()}.{extension}";
            var result = await attachmentsStorage.AddImageAsync(file.OpenReadStream(), imageFullName);

            var imageEntity = new Image
            {
                Name = imageFullName,
                Hash = HashBytesToString(hashService.ComputeHash(file.OpenReadStream())),
                OriginalName = file.FileName,
                Extension = extension,
                Width = result.Image.Width,
                Height = result.Image.Height,
                ThumbnailHeight = result.Thumbnail.Height,
                ThumbnailWidth = result.Thumbnail.Width,
                FileSize = (int)file.Length
            };
            return imageEntity;
        }

        public Task SetIsPinned(int threadId, bool isPinned)
        {
            var thread = new Thread { Id = threadId, IsPinned = isPinned };
            context.Attach(thread).Property(x => x.IsPinned).IsModified = true;
            return context.SaveChangesAsync();
        }

        public Task SetIsReadOnly(int threadId, bool isReadOnly)
        {
            var thread = new Thread { Id = threadId, IsReadOnly = isReadOnly };
            context.Attach(thread).Property(x => x.IsReadOnly).IsModified = true;
            return context.SaveChangesAsync();
        }
    }
}
