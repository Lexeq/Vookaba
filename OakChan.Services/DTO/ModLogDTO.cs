using OakChan.Common;
using System;
using System.Net;

namespace OakChan.Services.DTO
{
    public class ModLogDto
    {
        public ApplicationEvent EventId { get; set; }

        public string EntityId { get; set; }

        public DateTime Created { get; set; }

        public IPAddress IP { get; set; }

        public string Note { get; set; }
    }
}
