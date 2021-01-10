using Microsoft.AspNetCore.Http;
using System;

namespace OakChan.Services.DTO
{
    public class ThreadCreationDto
    {
        public string Subject { get; set; }

        public PostCreationDto OpPost { get; set; }
    }
}
