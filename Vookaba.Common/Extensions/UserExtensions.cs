using Vookaba.Common;
using System.Security.Claims;

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
    }
}
