﻿using System;
using Vookaba.ViewModels.Common;

namespace Vookaba.ViewModels.Post
{
    public class PostViewModel
    {
        public int Id { get; set; }

        public bool IsOpening { get; set; }

        public bool OpMark { get; set; }

        public bool IsSaged { get; set; }

        public int Number { get; set; }

        public DateTime Date { get; set; }

        public string ThreadId { get; set; }

        public Guid AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string Tripcode { get; set; }

        public string Message { get; set; }

        public bool HasImage => Image != null;

        public ImageViewModel Image { get; set; }
    }
}
