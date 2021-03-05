using AutoMapper;
using Microsoft.AspNetCore.Http;
using OakChan.Common.Exceptions;
using OakChan.DAL;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public class DbPostService : IPostService
    {
        private readonly IAttachmentsStorage attachmentsStorage;
        private readonly IHashService hashService;
        private readonly OakDbContext context;
        private readonly IMapper mapper;
        private readonly ThrowHelper throwHelper;

        public DbPostService(IAttachmentsStorage attachmentsStorage,
            IHashService hashService,
            OakDbContext context,
            IMapper mapper,
            ThrowHelper throwHelper)
        {
            this.attachmentsStorage = attachmentsStorage;
            this.hashService = hashService;
            this.context = context;
            this.mapper = mapper;
            this.throwHelper = throwHelper;
        }

        public async Task<PostDto> CreatePost(ThreadDto threadDto, PostCreationDto postDto)
        {
            throwHelper.ThrowIfNull(threadDto, nameof(threadDto));
            throwHelper.ThrowIfNull(postDto, nameof(postDto));

            var thread = mapper.Map<Thread>(threadDto);
            if (thread.Id != 0 || context.Threads.Local.Any(t => t.Id == thread.Id))
            {
                context.Attach(thread);
            }

            var post = new Post
            {
                CreationTime = DateTime.UtcNow,
                Message = postDto.Message,
                Name = postDto.AuthorName,
                Thread = thread,
                AuthorId = postDto.AuthorId,
                AuthorIP = postDto.IP,
                AuthorUserAgent = postDto.UserAgent
            };

            if (postDto.Attachment != null)
            {
                post.Image = await CreateImageAsync(postDto.Attachment);
            }

            context.Posts.Add(post);
            await context.SaveChangesAsync();
            return mapper.Map<PostDto>(post);
        }

        private async Task<Image> CreateImageAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
            string imageFileName = $"{GenerateFileName()}.{extension}";
            var imageEntity = new Image
            {
                Name = imageFileName,
                Hash = hashService.ComputeHash(file.OpenReadStream()),
                OriginalName = file.FileName,
                Type = extension,
                UploadDate = DateTime.UtcNow
            };

            var image = await attachmentsStorage.AddImageAsync(file.OpenReadStream(), imageFileName);
            imageEntity.Width = image.Width;
            imageEntity.Height = image.Height;
            imageEntity.Size = (int)file.Length;
            return imageEntity;
        }

        private string GenerateFileName()
            => Guid.NewGuid().ToString("N");
    }
}
