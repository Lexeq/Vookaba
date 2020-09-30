using System;

namespace OakChan.Models
{
    public class PostCreationData
    {
        public PostCreationData(Guid authorId)
        {
            Author = authorId;
        }

        public string Subject { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public UploadImageData Image { get; set; }

        public Guid Author { get; private set; }
    }
}
