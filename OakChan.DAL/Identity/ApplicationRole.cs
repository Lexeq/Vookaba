using Microsoft.AspNetCore.Identity;

namespace OakChan.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole()
        { }

        public ApplicationRole(string roleName) : base(roleName)
        { }
    }
}
