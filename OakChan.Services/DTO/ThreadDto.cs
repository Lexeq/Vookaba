using System;
using System.Collections.Generic;
using System.Text;

namespace OakChan.Services.DTO
{
    public class ThreadDto
    {
        public string BoardId { get; set; }

        public int ThreadId { get; set; }

        public IEnumerable<PostDto> Posts { get; set; }
    }
}
