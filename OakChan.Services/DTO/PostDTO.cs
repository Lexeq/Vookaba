using System;
using System.Collections.Generic;
using System.Text;

namespace OakChan.Services.DTO
{
    public class PostDto
    {
        public int PostId { get; set; }

        public int ThreadId { get; set; }

        public bool IsOpening { get; set; }

        public Guid AuthorId { get; set; }

        public string AuthorName { get; set; }

        public DateTime Created { get; set; }

        public string Message { get; set; }

        public bool IsSaged { get; set; }

        public int PostNumber { get; set; }

        public ImageDto Image { get; set; }

    }
}
