﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OakChan.Identity
{
    public class ApplicationUserStore :
        UserStore<ApplicationUser, ApplicationRole, OakDbContext, int>,
        IUserInvitationStore<ApplicationUser>,
        IInvitationStore<Invitation>
    {
        private DbSet<Invitation> Invitations => Context.Set<Invitation>();

        private new ExtendedIdentityErrorDescriber ErrorDescriber => base.ErrorDescriber as ExtendedIdentityErrorDescriber;

        public ApplicationUserStore(OakDbContext context, ExtendedIdentityErrorDescriber describer = null)
            : base(context, new ExtendedIdentityErrorDescriber()) { }

        public virtual async Task<IdentityResult> ApplyInvitationAsync(ApplicationUser user, string invitationToken, CancellationToken cancellationToken)
        {
            var invitation = await FindInvitationAsync(invitationToken, cancellationToken);

            var validationResult = await ValidateInvitationAsync(invitation, cancellationToken);

            if (!validationResult.Succeeded)
            {
                return validationResult;
            }

            invitation.UsedBy = user;
            invitation.ConcurrencyStamp = Guid.NewGuid().ToString();
            return IdentityResult.Success;
        }

        protected virtual async Task<Invitation> FindInvitationAsync(string invitationToken, CancellationToken cancellationToken)
        {
            return Guid.TryParse(invitationToken, out var id) ?
                await Invitations.FindAsync(new object[] { id }, cancellationToken) :
                null;
        }

        private Task<IdentityResult> ValidateInvitationAsync(Invitation invitation, CancellationToken cancellation)
        {
            List<IdentityError> errors = new();
            if (invitation == null)
            {
                errors.Add(ErrorDescriber.NoInvitation());
            }
            else if (invitation.UsedByID.HasValue)
            {
                errors.Add(ErrorDescriber.InvitationUsed());
            }
            else if (invitation.Expire < DateTime.UtcNow)
            {
                errors.Add(ErrorDescriber.InvitationExpired(invitation.Expire));
            }

            return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));

        }

        public override int ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default;
            }
            return int.Parse(id);
        }

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            Context.Add(user);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(base.ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> CreateAsync(Invitation invitation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (invitation == null)
            {
                throw new ArgumentNullException(nameof(invitation));
            }
            Context.Add(invitation);
            await SaveChanges(cancellationToken);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(Invitation invitation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (invitation == null)
            {
                throw new ArgumentNullException(nameof(invitation));
            }
            Context.Attach(invitation);
            invitation.ConcurrencyStamp = Guid.NewGuid().ToString();
            Context.Update(invitation);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(base.ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(Invitation invitation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (invitation == null)
            {
                throw new ArgumentNullException(nameof(invitation));
            }
            Context.Remove(invitation);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(base.ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success; ;
        }

        public virtual Task<string> GetInvitationTokenAsync(Invitation invitation, CancellationToken cancellationToken)
        {
            return Task.FromResult(invitation.Token);
        }

        public virtual Task<Invitation> FindInvitationByTokenAsync(string token, CancellationToken cancellationToken)
               => FindInvitationByIdAsync(token, cancellationToken);

        public virtual Task<Invitation> FindInvitationByIdAsync(string id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Invitations.FindAsync(new object[] { id }, cancellationToken).AsTask();
        }
    }
}
