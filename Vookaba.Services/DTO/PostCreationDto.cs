using Microsoft.AspNetCore.Http;

namespace Vookaba.Services.DTO
{
    public class PostCreationDto
    {
        public string? AuthorName { get; set; }

        public string? Message { get; set; }

        public string? EncodedMessage { get; set; }

        public IFormFileCollection? Attachments { get; set; }

        public bool IsSaged { get; set; }

        public bool OpMark { get; set; }

        public string? Tripcode { get; set; }
    }
}
