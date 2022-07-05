using Microsoft.AspNetCore.Http;
using System;

namespace Vookaba.Services.DTO
{
    public class ThreadCreationDto
    {
        public string Subject { get; set; }

        public PostCreationDto OpPost { get; set; }
    }
}
