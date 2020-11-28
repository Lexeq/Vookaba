using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class ThreadViewModel
    {
        public string Board { get; set; }

        public int Id { get; set; }

        public PostFormViewModel Post { get; set; }

        public IEnumerable<PostViewModel> Posts { get; set; }

    }
}
