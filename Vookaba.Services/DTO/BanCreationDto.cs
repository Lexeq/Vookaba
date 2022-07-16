using System.Net;

namespace Vookaba.Services.DTO
{
    public class BanCreationDto
    {
        public (IPAddress Address, int Subnet)? BannedNetwork { get; set; }

        public Guid? BannedAothorToken { get; set; }

        public string? Board { get; set; }

        public string? Reason { get; set; }

        public DateTime Expired { get; set; }
    }

}
