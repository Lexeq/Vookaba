using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Vookaba.Common;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vookaba.Identity
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
                     RoleManager<ApplicationRole> roleManager,
                     IOptions<IdentityOptions> identityOptions)
            : base(userManager, roleManager, identityOptions) { }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);
            claimsIdentity.AddClaim(new Claim(ApplicationConstants.ClaimTypes.AuthorToken, user.AuthorTokenId.ToString()));
            return claimsIdentity;
        }
    }
}
