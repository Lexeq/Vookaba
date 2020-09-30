using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OakChan.Models
{
    public class MediaStorage
    {
        private const string ThumbnailSuffix = "-min";

        private readonly string imagesFolder;
        private readonly string rootFolder;


        public MediaStorage(IWebHostEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }
            rootFolder = environment.WebRootPath;
            imagesFolder = Path.Combine("res", "img");
        }


        public async Task<Image> AddImage(byte[] bytes, string name)
        {
            using Image im = Image.Load(bytes);
            var imagePath = Path.Combine(rootFolder, imagesFolder, name);
            var thumbPath = Path.Combine(rootFolder, imagesFolder, GetThumbnailName(name));
            await im.SaveAsync(imagePath);
            await CreateThumbnail(im).SaveAsync(thumbPath);
            return im;
        }

        public string GetImageRelativePath(string name)
            => Path.Combine(imagesFolder, name);
        public string GetImageThumbnailRelativePath(string name)
            => Path.Combine(imagesFolder, GetThumbnailName(name));

        private string GetThumbnailName(string name)
            => Path.GetFileNameWithoutExtension(name) + ThumbnailSuffix + Path.GetExtension(name);

        public Image CreateThumbnail(Image image)
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
