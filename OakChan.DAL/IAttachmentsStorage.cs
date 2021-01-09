using System.IO;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public interface IAttachmentsStorage
    {
        string GetThumbnailLinkByName(string imageName);

        string GetImageLinkByName(string imageName);

        Task<ImageInfo> AddImageAsync(byte[] bytes, string imageName);

        Task<ImageInfo> AddImageAsync(Stream source, string imageName);
    }
}
