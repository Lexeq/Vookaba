using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Identity
{
    public class UserManagerWithInvitations<TUser> : UserManager<TUser> where TUser : class
    {
        public virtual bool SupportsInvitations
        {
            get
            {
                ThrowIfDisposed();
                return Store is IUserInvitationStore<TUser>;
            }
        }

        public UserManagerWithInvitations(IUserStore<TUser> store,
          IOptions<IdentityOptions> optionsAccessor,
          IPasswordHasher<TUser> passwordHasher,
          IEnumerable<IUserValidator<TUser>> userValidators,
          IEnumerable<IPasswordValidator<TUser>> passwordValidators,
          ILookupNormalizer keyNormalizer,
          IdentityErrorDescriber errors,
          IServiceProvider services,
          ILogger<UserManagerWithInvitations<TUser>> logger) : base(store, optionsAccessor, passwordHasher,
                                                               userValidators, passwordValidators, keyNormalizer,
                                                               errors, services, logger)
        {
        }

        public async Task<IdentityResult> CreateAsync(TUser user, string password, string invitationToken)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var invitationStore = GetInvitationStore();
            if (invitationToken == null)
            {
                throw new ArgumentNullException(nameof(invitationToken));
            }

            IdentityResult identityResult = await UseInvitation(invitationStore, user, invitationToken);
            if (!identityResult.Succeeded)
            {
                return identityResult;
            }

            return await CreateAsync(user, password);
        }

        private IUserInvitationStore<TUser> GetInvitationStore()
        {
            return (Store as IUserInvitationStore<TUser>) ?? throw new NotSupportedException("Store is not IInvitationStore");
        }

        private async Task<IdentityResult> UseInvitation(IUserInvitationStore<TUser> invitationStore, TUser user, string invitationToken)
        {
            var identityResult = await invitationStore.ApplyInvitationAsync(user, invitationToken, CancellationToken);

            if (!identityResult.Succeeded)
            {
                Logger.LogWarning("User invitation applying failed: {errors}.", string.Join(";", identityResult.Errors.Select((IdentityError e) => e.Code)));
                return IdentityResult.Failed(identityResult.Errors.ToArray());
            }
            return IdentityResult.Success; ;
        }

    }

}
