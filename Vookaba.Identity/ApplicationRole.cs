using Microsoft.AspNetCore.Identity;

namespace Vookaba.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole()
        { }

        public ApplicationRole(string roleName) : base(roleName)
        { }
    }
}
