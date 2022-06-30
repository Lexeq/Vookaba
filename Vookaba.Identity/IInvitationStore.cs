using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vookaba.Identity
{
    public interface IInvitationStore<TInvitation> : IDisposable where TInvitation : class
    {
        Task<IdentityResult> CreateAsync(TInvitation invitation, CancellationToken cancellationToken);

        Task<IdentityResult> UpdateAsync(TInvitation invitation, CancellationToken cancellationToken);

        Task<IdentityResult> DeleteAsync(TInvitation invitation, CancellationToken cancellationToken);

        Task<string> GetInvitationTokenAsync(TInvitation invitation, CancellationToken cancellationToken);

        Task<TInvitation> FindInvitationByTokenAsync(string token, CancellationToken cancellationToken);

        Task<TInvitation> FindInvitationByIdAsync(string id, CancellationToken cancellationToken);

    }
}
