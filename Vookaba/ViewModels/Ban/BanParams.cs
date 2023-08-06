#nullable enable

namespace Vookaba.ViewModels.Ban
{
    public class BanParams
    {
        public required bool IsIPBan { get; set; }
        public required bool IsSubnetBan { get; set; }
        public required string Board { get; set; }
    }
}
