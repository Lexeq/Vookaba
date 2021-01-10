using SixLabors.ImageSharp;
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

        public int ThumbnailMaxSize => 240;

        public MediaStorage(string rootPath)
        {
            rootFolder = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        }

        public Task<ImageInfo> AddImageAsync(byte[] bytes, string imageName)
            => AddImageAsync(Image.Load(bytes), imageName);

        public Task<ImageInfo> AddImageAsync(Stream source, string imageName)
            => AddImageAsync(Image.Load(source), imageName);

        private async Task<ImageInfo> AddImageAsync(Image image, string name)
        {
            var imagePath = Path.Combine(rootFolder, MediaResourcesFolder, ImagesFolder, name);
            var thumbName = GetThumbnailName(name);
            var thumbPath = Path.Combine(rootFolder, MediaResourcesFolder, ImagesFolder, thumbName);
            await image.SaveAsync(imagePath);
            var thumb = CreateThumbnail(image);
            await thumb.SaveAsync(thumbPath);
            return new ImageInfo
            {
                Height = image.Height,
                Name = name,
                Width = image.Width,
                Thumbnail = new ImageInfo
                {
                    Height = thumb.Height,
                    Name = GetThumbnailName(name),
                    Width = thumb.Width
                }
            };
        }

        public string GetImageLinkByName(string name)
            => $"/{MediaResourcesFolder}/{ImagesFolder}/{name}";

        public string GetThumbnailLinkByName(string name)
            => GetImageLinkByName(GetThumbnailName(name));

        private string GetThumbnailName(string name)
            => Path.GetFileNameWithoutExtension(name) + ThumbnailSuffix + Path.GetExtension(name);

        private Image CreateThumbnail(Image image)
        {
            var thumbSource = image.Frames.Count > 1 ? image.Frames.ExportFrame(0) : image;

            return thumbSource.Clone(opt => opt.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.Max,
                Size = new Size(ThumbnailMaxSize)
            }));
        }
    }
}
