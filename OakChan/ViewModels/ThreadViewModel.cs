using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class ThreadViewModel
    {
        public string BoardId { get; set; }

        public int ThreadId { get; set; }

        public PostFormViewModel Post { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }

    }
}
