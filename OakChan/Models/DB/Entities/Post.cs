using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ThreadId { get; set; }

        public DateTime CreationTime { get; set; }

        public string Message { get; set; }

        public Guid UserId { get; set; }

        public Image Image { get; set; }

        public string Subject { get; set; }
    }
}
