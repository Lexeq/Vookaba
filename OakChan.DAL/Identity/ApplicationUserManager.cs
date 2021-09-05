using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OakChan.DAL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Identity
{
    public class ApplicationUserManager : UserManagerWithInvitations<ApplicationUser>
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
                                      ILogger<ApplicationUserManager> logger) : base(
                                          store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
                                          keyNormalizer, errors, services, logger)
        {
            this.authorTokens = authorTokens;
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            ThrowIfDisposed();
            var aToken = await authorTokens.GenerateTokenAsync();
            user.AuthorToken = aToken;
            return await base.CreateAsync(user);
        }
    }

}
