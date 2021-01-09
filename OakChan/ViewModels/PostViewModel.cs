using System;

namespace OakChan.ViewModels
{
    public class PostViewModel
    {
        public int Number { get; set; }

        public DateTime Date { get; set; }

        public string ThreadId { get; set; }

        public Guid AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string Message { get; set; }

        public bool HasImage => Image != null;

        public ImageViewModel Image { get; set; }
    }
}
