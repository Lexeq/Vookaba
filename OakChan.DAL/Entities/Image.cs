using OakChan.DAL.Entities.Base;

namespace OakChan.DAL.Entities
{

    public class Image : Attachment
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}
