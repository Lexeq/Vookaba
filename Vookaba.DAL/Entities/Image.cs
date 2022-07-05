using Vookaba.DAL.Entities.Base;

namespace Vookaba.DAL.Entities
{

    public class Image : Attachment
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
    }
}
