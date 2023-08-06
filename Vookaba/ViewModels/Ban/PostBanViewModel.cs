#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Vookaba.ViewModels.Ban
{
    public class PostBanViewModel
    {
        [Required]
        public required string Duration { get; set; }

        public bool AllBoard { get; set; }
    }
}
