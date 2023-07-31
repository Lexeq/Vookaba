#nullable enable
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Vookaba.ViewModels.Ban
{
    public class BanCreationViewModel
    {
        [Required]
        public required string Reason { get; set; }

        [Required]
        public required string Duration { get; set; }

        [Required]
        public required string Board { get; set; }

        public int Token { get; set; }

        public IPAddress? IP { get; set; }

        public int? Subnet { get; set; }
    }
}
