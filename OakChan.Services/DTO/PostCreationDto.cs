using Microsoft.AspNetCore.Http;

namespace OakChan.Services.DTO
{
    public class PostCreationDto
    {
        public string AuthorName { get; set; }

        public string Message { get; set; }

        public IFormFileCollection Attachments { get; set; }

        public bool IsSaged { get; set; }

        public bool OpMark { get; set; }
    }
}
