using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vookaba.Identity
{
    public interface IAuthorTokenStore<TAuthorToken> : IDisposable
        where TAuthorToken : class
    {
        public Task<IdentityResult> CreateAsync(TAuthorToken authorToken, CancellationToken cancellationToken);

        public Task<IdentityResult> DeleteAsync(TAuthorToken authorToken, CancellationToken cancellationToken);
    }
}
