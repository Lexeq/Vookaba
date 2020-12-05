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

        public MediaStorage(string rootPath)
        {
            rootFolder = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
        }

        public async Task<Image> AddImage(byte[] bytes, string name)
        {
            using Image im = Image.Load(bytes);
            var imagePath = Path.Combine(rootFolder, MediaResourcesFolder, ImagesFolder, name);
            var thumbPath = Path.Combine(rootFolder, MediaResourcesFolder, ImagesFolder, GetThumbnailName(name));
            await File.WriteAllBytesAsync(imagePath, bytes);
            await CreateThumbnail(im).SaveAsync(thumbPath);
            return im;
        }

        public string GetImageLinkByName(string name)
            => $"/{MediaResourcesFolder}/{ImagesFolder}/{name}";

        public string GetThumbnailLinkByName(string name)
            => GetImageLinkByName(GetThumbnailName(name));

        private string GetThumbnailName(string name)
            => Path.GetFileNameWithoutExtension(name) + ThumbnailSuffix + Path.GetExtension(name);

        private Image CreateThumbnail(Image image)
        {
            const int thumbnailMaxSize = 240;
            var thumbSource = image.Frames.Count > 1 ? image.Frames.ExportFrame(0) : image;

            return thumbSource.Clone(opt => opt.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.Max,
                Size = new Size(thumbnailMaxSize)
            }));
        }
    }
}
