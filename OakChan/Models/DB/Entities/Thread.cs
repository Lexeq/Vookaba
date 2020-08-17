using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB.Entities
{
    public class Thread
    {
        public int Id { get; set; }

        public string BoardId { get; set; }

        public IList<Post> Posts { get; set; }
    }
}
