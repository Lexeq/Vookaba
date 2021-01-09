using System.Collections.Generic;

namespace OakChan.Services.DTO
{
    public class ThreadDto
    {
        public string BoardId { get; set; }

        public int ThreadId { get; set; }

        public string Subject { get; set; }

        public PostDto OpPost { get; set; }

        public IEnumerable<PostDto> Replies { get; set; }
    }
}
