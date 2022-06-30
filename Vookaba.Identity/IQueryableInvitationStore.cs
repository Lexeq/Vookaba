using System;
using System.Linq;

namespace Vookaba.Identity
{
    public interface IQueryableInvitationStore<TInvitation> : IInvitationStore<TInvitation>, IDisposable where TInvitation : class
    {
        IQueryable<TInvitation> Invitations { get; }
    }

}
