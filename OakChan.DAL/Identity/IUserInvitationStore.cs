using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OakChan.Identity
{
    public interface IUserInvitationStore<TUser> : IDisposable where TUser : class
    {
        Task<IdentityResult> ApplyInvitationAsync(TUser user, string invitation, CancellationToken cancellationToken);
    }

}
