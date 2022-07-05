using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vookaba.Identity;

namespace Vookaba.DAL.Identity
{
    public class ApplicationRoleStore<TContext> : RoleStore<ApplicationRole, TContext, int>
        where TContext : DbContext
    {
        public ApplicationRoleStore(TContext context, IdentityErrorDescriber describer = null)
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
