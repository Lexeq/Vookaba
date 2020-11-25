using System.Collections.Generic;

namespace OakChan.Services.DTO
{
    public class BoardPreview
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public int TotalThreadsCount { get; set; }

        public IEnumerable<ThreadPreview> Threads { get; set; }
    }
}
