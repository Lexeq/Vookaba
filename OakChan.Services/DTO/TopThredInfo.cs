using OakChan.DAL.Entities;

namespace OakChan.Services.DTO
{
    public class TopThredInfo
    {
        public Post OpPost { get; set; }

        public int PostsCount { get; set; }

        public string BoardId { get; set; }

        public int ThreadId { get; set; }
    }
}
