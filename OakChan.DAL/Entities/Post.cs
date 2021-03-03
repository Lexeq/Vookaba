using System;

namespace OakChan.DAL.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Thread Thread { get; set; }

        public int ThreadId { get; set; }

        public DateTime CreationTime { get; set; }

        public string Message { get; set; }

        public Guid AuthorId { get; set; }

        public Image Image { get; set; }
    }
}
