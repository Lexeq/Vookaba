using System;

namespace OakChan.DAL.Entities
{
    public class Image
    {
        public int Id { get; set; }

        public string OriginalName { get; set; }

        public string Type { get; set; }

        public DateTime UploadDate { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Size { get; set; }

        public byte[] Hash { get; set; }

        public string Name { get; set; }
    }
}
