using System.Collections.Generic;

namespace OakChan.Services.DTO
{
    public class BoardPageDto
    {
        public string BoardId { get; set; }

        public int PageNumber { get; set; }

        public ICollection<ThreadPreviewDto> Threads { get; set; }
    }
}
