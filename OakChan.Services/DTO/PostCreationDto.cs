using Microsoft.AspNetCore.Http;
using System;

namespace OakChan.Services.DTO
{
    public class PostCreationDto
    {
        public Guid AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string Message { get; set; }

        public IFormFile Attachment { get; set; }
    }
}
