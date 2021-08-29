using OakChan.DAL.Entities.Base;
using System;
using System.Collections.Generic;

namespace OakChan.DAL.Entities
{
    public class Thread : IHasCreationTime
    {
        public int Id { get; set; }

        public string BoardKey { get; set; }

        public Board Board { get; set; }

        public string Subject { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsPinned { get; set; }

        public DateTime LastBump { get; set; }

        public DateTime LastHit { get; set; }

        public int PostsCount { get; set; }

        public int PostsWithAttachmentnsCount { get; set; }

        public IList<Post> Posts { get; set; }

        public DateTime Created { get; set; }
    }
}
