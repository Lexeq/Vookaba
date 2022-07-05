namespace Vookaba.DAL.Entities.Base
{
    public abstract class Attachment
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }

        public string OriginalName { get; set; }

        public string Extension { get; set; }

        public string Hash { get; set; }

        public int FileSize { get; set; }

        public string Name { get; set; }
    }
}
