using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OakChan.DAL.Database;

namespace OakChan.Identity
{
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, OakDbContext, int, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>
    {
        public ApplicationUserStore(OakDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer) { }

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
