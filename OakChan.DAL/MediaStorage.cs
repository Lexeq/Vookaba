using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public class MediaStorage : IAttachmentsStorage
    {
        private const string ThumbnailSuffix = "-min";
        private const string MediaResourcesFolder = "res";
        private const string ImagesFolder = "img";

        private readonly string rootFolder;
        private readonly ILogger<MediaStorage> logger;

        public int ThumbnailMaxSize => 240;

        public MediaStorage(string rootPath, ILogger<MediaStorage> logger)
        {
            rootFolder = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            this.logger = logger;
        }

        public Task<ImageSavingResult> AddImageAsync(byte[] bytes, string imageName)
        {
            using var image = Image.Load(bytes);
            return AddImageAsync(image, imageName);
        }

        public async Task<ImageSavingResult> AddImageAsync(Stream source, string imageName)
        {
            using var image = await Image.LoadAsync(source);
            return await AddImageAsync(image, imageName);
        }

        private async Task<ImageSavingResult> AddImageAsync(Image image, string name)
        { 
            var imagePath = GetImagePath(name);
            var thumbPath = GetThumbnailPath(name);
            await image.SaveAsync(imagePath);
            var thumb = CreateThumbnail(image);
            await thumb.SaveAsync(thumbPath);


            var result = new ImageSavingResult
            {
                Image = new ImageInfo
                {
                    Height = image.Height,
                    Name = name,
                    Width = image.Width
                },
                Thumbnail = new ImageInfo
                {
                    Height = thumb.Height,
                    Name = GetThumbnailName(name),
                    Width = thumb.Width
                }
            };

            return result;
        }

        public string GetImageLinkByName(string name)
            => $"/{MediaResourcesFolder}/{ImagesFolder}/{name}";

        public string GetThumbnailLinkByName(string name)
            => GetImageLinkByName(GetThumbnailName(name));

        public Task DeleteImageAsync(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            void Delete(string path)
            {
                if (File.Exists(GetImagePath(path)))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, $"Failed to delete file: {path}");
                    }
                }
            }

            Delete(GetImagePath(name));
            Delete(GetThumbnailPath(name));
            return Task.CompletedTask;
        }



        private Image CreateThumbnail(Image image)
        {
            var thumbSource = image.Frames.Count > 1 ? image.Frames.ExportFrame(0) : image;

            return thumbSource.Clone(opt => opt.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.Max,
                Size = new Size(ThumbnailMaxSize)
            }));
        }

        private string GetImagePath(string name)
        {
            return Path.Combine(rootFolder, MediaResourcesFolder, ImagesFolder, name);
        }
        private string GetThumbnailPath(string name)
        {
            var thumbName = GetThumbnailName(name);
            return GetImagePath(thumbName);
        }

        private string GetThumbnailName(string name)
            => Path.GetFileNameWithoutExtension(name) + ThumbnailSuffix + Path.GetExtension(name);
    }
}
