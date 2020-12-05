using System;
using System.Collections.Generic;
using System.Text;

namespace OakChan.DAL
{
    public interface IAttachmentsStorage
    {
        string GetThumbnailLinkByName(string imageName);

        string GetImageLinkByName(string imageName);
    }
}
