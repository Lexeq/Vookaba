using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace OakChan.Services.DTO
{
    public class PostCreationDto
    {
        public Guid AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string Message { get; set; }

        public IFormFileCollection Attachments { get; set; }

        public IPAddress IP { get; set; }

        public string UserAgent { get; set; }

        public bool IsSaged { get; set; }
    }
}
