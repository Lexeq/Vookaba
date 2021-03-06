using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OakChan.DAL;
using OakChan.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OakChan.Deanon
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly DeanonOptions deanonOptions;
        private readonly IdTokenManager idTokens;

        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
                     RoleManager<ApplicationRole> roleManager,
                     IOptions<IdentityOptions> identityOptions,
                     IOptions<DeanonOptions> deanonOptions,
                     IdTokenManager IdTokens)
            : base(userManager, roleManager, identityOptions)
        {
            this.deanonOptions = deanonOptions.Value;
            this.idTokens = IdTokens;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);
            var tok = await idTokens.GetUserToken(user.Id);
            claimsIdentity.AddClaim(new Claim(deanonOptions.IdTokenClaimType, tok.Id.ToString()));
            return claimsIdentity;
        }
    }
}
