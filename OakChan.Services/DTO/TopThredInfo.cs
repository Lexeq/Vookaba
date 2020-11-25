using OakChan.DAL.Entities;
using OakChan.Models.DB.Entities;

namespace OakChan.Models
{
    public class TopThredInfo
    {
        public Post OpPost { get; set; }

        public int PostsCount { get; set; }

        public string BoardId { get; set; }

        public int ThreadId { get; set; }
    }
}
