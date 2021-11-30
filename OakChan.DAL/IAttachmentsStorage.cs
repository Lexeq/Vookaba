using System.IO;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public interface IAttachmentsStorage
    {
        string GetThumbnailLinkByName(string imageName);

        string GetImageLinkByName(string imageName);

        Task<ImageSavingResult> AddImageAsync(byte[] bytes, string imageName);

        Task<ImageSavingResult> AddImageAsync(Stream source, string imageName);

        Task DeleteImageAsync(string imageName);
    }
}
