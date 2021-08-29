using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OakChan.DAL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private readonly IAuthorTokenFactory authorTokens;

        public ApplicationUserManager(IUserStore<ApplicationUser> store,
                  IOptions<IdentityOptions> optionsAccessor,
                  IPasswordHasher<ApplicationUser> passwordHasher,
                  IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                  IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
                  ILookupNormalizer keyNormalizer,
                  IdentityErrorDescriber errors,
                  IServiceProvider services,
                  IAuthorTokenFactory authorTokens,
                  ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher,
                                                                       userValidators, passwordValidators, keyNormalizer,
                                                                       errors, services, logger)
        {
            this.authorTokens = authorTokens;
        }
        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            if (user.AuthorToken == default)
            {
                var aToken = await authorTokens.CreateTokenAsync();
                user.AuthorToken = aToken.Token;
            }
            return await base.CreateAsync(user);
        }
    }
}
