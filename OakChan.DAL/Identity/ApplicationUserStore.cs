using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OakChan.DAL;
using OakChan.DAL.Database;
using System.Threading;
using System.Threading.Tasks;

namespace OakChan.Identity
{
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, OakDbContext, int, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationUserToken, ApplicationRoleClaim>
    {
        private readonly IdTokenManager tokens;

        public ApplicationUserStore(OakDbContext context,
            IdTokenManager tokens,
            IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
            this.tokens = tokens;
        }

        public override int ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default;
            }
            return int.Parse(id);
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var baseResult = await base.CreateAsync(user, cancellationToken);
            if (!baseResult.Succeeded)
            {
                return baseResult;
            }
            await tokens.CreateUserTokenAsync(user.Id);
            return IdentityResult.Success;
        }
    }
}
