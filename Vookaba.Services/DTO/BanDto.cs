using System.Net;

namespace Vookaba.Services.DTO
{
    public class BanDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Created { get; set; }

        public (IPAddress Address, int Subnet)? BannedNetwork { get; set; }

        public DateTime Expired { get; set; }

        public string? Reason { get; set; }

        public Guid? BannedAothorToken { get; set; }

        public string? Board { get; set; }

        public bool IsCanceled { get; set; }
    }
}
