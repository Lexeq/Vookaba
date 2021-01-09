using System.Collections.Generic;

namespace OakChan.DAL.Entities
{
    public class Thread
    {
        public int Id { get; set; }

        public string BoardId { get; set; }

        public Board Board { get; set; }

        public string Subject { get; set; }

        public IList<Post> Posts { get; set; }
    }
}
