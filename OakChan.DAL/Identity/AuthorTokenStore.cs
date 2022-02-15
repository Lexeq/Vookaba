using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OakChan.Identity
{
    public class AuthorTokenStore<TAuthorToken, TContext> : IAuthorTokenStore<TAuthorToken>
        where TAuthorToken : AuthorToken, new()
        where TContext : DbContext
    {
        private readonly TContext context;

        private bool _disposed;

        private DbSet<TAuthorToken> AuthorTokens => context.Set<TAuthorToken>();

        public AuthorTokenStore(TContext context)
        {
            this.context = context;
        }

        public async Task<IdentityResult> CreateAsync(TAuthorToken authorToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (authorToken == null)
            {
                throw new ArgumentNullException(nameof(authorToken));
            }
            AuthorTokens.Add(authorToken);
            await context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TAuthorToken authorToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (authorToken == null)
            {
                throw new ArgumentNullException(nameof(authorToken));
            }
            AuthorTokens.Remove(authorToken);

            await context.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
