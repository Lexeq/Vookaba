using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class ThreadViewModel
    {
        public string BoardKey { get; set; }

        public int ThreadId { get; set; }

        public string Subject { get; set; }

        public PostViewModel OpPost { get; set; }

        public IEnumerable<PostViewModel> Replies { get; set; }
    }
}
