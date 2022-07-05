using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vookaba.Identity
{
    public class ApplicationUserManager : UserManagerWithInvitations<ApplicationUser>
    {
        private readonly IAuthorTokenFactory<AuthorToken> authorTokenFactory;
        private readonly IAuthorTokenStore<AuthorToken> authorTokensStore;

        public ApplicationUserManager(IUserStore<ApplicationUser> store,
                                      IOptions<IdentityOptions> optionsAccessor,
                                      IPasswordHasher<ApplicationUser> passwordHasher,
                                      IEnumerable<IUserValidator<ApplicationUser>> userValidators,
                                      IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
                                      ILookupNormalizer keyNormalizer,
                                      IdentityErrorDescriber errors,
                                      IServiceProvider services,
                                      IAuthorTokenFactory<AuthorToken> authorTokenFactory,
                                      IAuthorTokenStore<AuthorToken> authorTokensStore,
                                      ILogger<ApplicationUserManager> logger) : base(
                                          store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
                                          keyNormalizer, errors, services, logger)
        {
            this.authorTokenFactory = authorTokenFactory;
            this.authorTokensStore = authorTokensStore;
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            ThrowIfDisposed();
            var aToken = await authorTokenFactory.GenerateTokenAsync();
            user.AuthorToken = aToken;
            return await base.CreateAsync(user);
        }

        public virtual async Task<Guid> CreateAnonymousTokenAsync()
        {
            var token = await authorTokenFactory.GenerateTokenAsync();
            await authorTokensStore.CreateAsync(token, CancellationToken);
            return token.Token;
        }
    }

}
