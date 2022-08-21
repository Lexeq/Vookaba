using Vookaba.Common;
using System.Security.Claims;
using System.Linq;

namespace Vookaba.Utils
{
    public static class UserExtensions
    {
        public static bool HasBoardPermission(this ClaimsPrincipal principal, string board)
        {
            if (principal.IsInRole(ApplicationConstants.Roles.Administrator))
            {
                return true;
            }
            else if (principal.IsInRole(ApplicationConstants.Roles.Janitor) || principal.IsInRole(ApplicationConstants.Roles.Moderator))
            {
                if (principal.HasClaim(ApplicationConstants.ClaimTypes.BoardPermission, board))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsRegistred(this ClaimsPrincipal principal)
        {
            return principal.Identity.IsAuthenticated && principal.Identities.Any(x => x.HasClaim(c => c.Type == ClaimTypes.NameIdentifier));
        }
    }
}
