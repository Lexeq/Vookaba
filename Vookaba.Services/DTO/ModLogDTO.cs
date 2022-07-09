using Vookaba.Common;
using System.Net;

namespace Vookaba.Services.DTO
{
    public class ModLogDto
    {
        public ApplicationEvent EventId { get; set; }

        public string EntityId { get; set; } = null!;

        public DateTime Created { get; set; }

        public IPAddress IP { get; set; } = null!;

        public string? Note { get; set; }
    }
}
