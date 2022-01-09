using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OakChan.Common.Exceptions;
using OakChan.DAL;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using OakChan.DAL.Entities.Base;
using OakChan.Services.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Services.DbServices
{
    public class DbThreadService : IThreadService
    {
        private readonly OakDbContext context;
        private readonly IAttachmentsStorage attachmentsStorage;
        private readonly IHashService hashService;
        private readonly IMapper mapper;
        private readonly ThrowHelper throwHelper;

        public DbThreadService(OakDbContext context, IAttachmentsStorage attachmentsStorage, IHashService hashService, IMapper mapper, ThrowHelper throwHelper)
        {
            this.context = context;
            this.attachmentsStorage = attachmentsStorage;
            this.hashService = hashService;
            this.mapper = mapper;
            this.throwHelper = throwHelper;
        }

        public async Task<ThreadDto> GetThreadAsync(string boardKey, int threadId)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));

            var thread = await context.Threads.AsNoTracking()
                .Where(t => t.BoardKey == boardKey && t.Id == threadId)
                .Include(t => t.Posts.OrderBy(p => p.Number))
                .ThenInclude(p => p.Attachments)
                .FirstOrDefaultAsync();

            return mapper.Map<ThreadDto>(thread);
        }

        public async Task<ThreadDto> CreateThreadAsync(string boardKey, ThreadCreationDto threadDto)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));
            throwHelper.ThrowIfNull(threadDto, nameof(threadDto));

            var thread = new Thread { BoardKey = boardKey, Subject = threadDto.Subject };
            var post = await CreatePostEntityAsync(threadDto.OpPost);
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

        public async Task<PostDto> AddPostToThreadAsync(string boardKey, int threadId, PostCreationDto postData)
        {
            throwHelper.ThrowIfNullOrWhiteSpace(boardKey, nameof(boardKey));
            throwHelper.ThrowIfNull(postData, nameof(postData));

            var post = await CreatePostEntityAsync(postData);
            post.ThreadId = threadId;

            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return mapper.Map<PostDto>(post);
        }

        private async Task<Post> CreatePostEntityAsync(PostCreationDto postDto)
        {
            throwHelper.ThrowIfNull(postDto, nameof(postDto));

            var post = mapper.Map<Post>(postDto);
            if (postDto.Attachments != null && postDto.Attachments.Any())
            {
                post.Attachments = await CreateImageEntityAsync(postDto.Attachments);
            }

            return post;
        }

        //TODO: Support different attachment types
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
                FileSize = (int)file.Length
            };
            return imageEntity;
        }
    }
}
