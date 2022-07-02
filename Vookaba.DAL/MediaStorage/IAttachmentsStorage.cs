using System.IO;
using System.Threading.Tasks;

namespace Vookaba.DAL.MediaStorage
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
