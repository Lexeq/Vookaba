using OakChan.Common;
using System.Security.Claims;

namespace OakChan.Utils
{
    public static class UserExt
    {
        public static bool HasBoardPermission(this ClaimsPrincipal principal, string board)
        {
            if (principal.IsInRole(OakConstants.Roles.Administrator))
            {
                return true;
            }
            else if (principal.IsInRole(OakConstants.Roles.Janitor) || principal.IsInRole(OakConstants.Roles.Moderator))
            {
                if (principal.HasClaim(OakConstants.ClaimTypes.BoardPermission, board))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
