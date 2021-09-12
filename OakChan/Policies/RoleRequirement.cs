using Microsoft.AspNetCore.Authorization;

namespace OakChan.Policies
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string RoleName { get; }

        public RoleRequirement(string roleName)
        {
            RoleName = roleName;
        }
    }
}
