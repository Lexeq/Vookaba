using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OakChan.DAL.Database;

namespace OakChan.Identity
{
    public class ApplicationRoleStore : RoleStore<ApplicationRole, OakDbContext, int>
    {
        public ApplicationRoleStore(OakDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        { }

        public override int ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default;
            }
            return int.Parse(id);
        }
    }
}
