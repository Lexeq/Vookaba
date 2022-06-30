using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vookaba.Identity
{
    public class InvitationManager<TInvitation> : IDisposable where TInvitation : class
    {
        private readonly ILogger<InvitationManager<TInvitation>> _logger;

        private bool _disposed;


        protected IInvitationStore<TInvitation> Store { get; private set; }

        protected virtual CancellationToken CancellationToken => CancellationToken.None;


        public virtual IQueryable<TInvitation> Invitations => ((Store as IQueryableInvitationStore<TInvitation>) ?? throw new NotSupportedException($"Store is not {nameof(IQueryableInvitationStore<TInvitation>)}.")).Invitations;

        public virtual bool SupportsQueryableInvitations
        {
            get
            {
                ThrowIfDisposed();
                return Store is IQueryableRoleStore<TInvitation>;
            }
        }

        public InvitationManager(IInvitationStore<TInvitation> store,
            ILogger<InvitationManager<TInvitation>> logger)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            _logger = logger;
        }

        public virtual async Task<IdentityResult> CreateAsync(TInvitation invitation)
        {
            ThrowIfDisposed();
            if (invitation == null)
            {
                throw new ArgumentNullException(nameof(invitation));
            }
            return await Store.CreateAsync(invitation, CancellationToken);
        }

        public virtual Task<IdentityResult> DeleteAsync(TInvitation invitation)
        {
            ThrowIfDisposed();
            if (invitation == null)
            {
                throw new ArgumentNullException(nameof(invitation));
            }
            return Store.DeleteAsync(invitation, CancellationToken);
        }

        public virtual Task<TInvitation> FindByTokenAsync(string token)
        {
            ThrowIfDisposed();
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            return Store.FindInvitationByTokenAsync(token, CancellationToken);
        }

        public virtual Task<TInvitation> FindByIdAsync(string invitationId)
        {
            ThrowIfDisposed();
            if (invitationId == null)
            {
                throw new ArgumentNullException(nameof(invitationId));
            }
            return Store.FindInvitationByIdAsync(invitationId, CancellationToken);
        }

        public virtual Task<string> GetInvitationTokenAsync(TInvitation invitation)
        {
            ThrowIfDisposed();
            if (invitation == null)
            {
                throw new ArgumentNullException(nameof(invitation));
            }
            return Store.GetInvitationTokenAsync(invitation, CancellationToken);
        }

        public virtual async Task<bool> InvitionExistsAsync(string token)
        {
            ThrowIfDisposed();
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            return await FindByTokenAsync(token) != null;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                Store.Dispose();
            }
            _disposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }

}
