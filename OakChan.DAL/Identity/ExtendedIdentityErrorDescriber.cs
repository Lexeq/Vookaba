﻿using Microsoft.AspNetCore.Identity;
using System;

namespace OakChan.Identity
{
    public class ExtendedIdentityErrorDescriber : IdentityErrorDescriber
    {
        public virtual IdentityError InvitationExpired(DateTime expired)
        {
            return new IdentityError
            {
                Code = "InvatationExpired",
                Description = $"The invitation expired on {expired}."
            };
        }

        public virtual IdentityError InvitationUsed()
        {
            return new IdentityError
            {
                Code = "InvatatioUsed",
                Description = $"The invitation has already been used ."
            };
        }

        public virtual IdentityError NoInvitation()
        {
            return new IdentityError
            {
                Code = "NoInvitation",
                Description = $"Invitation token was not found"
            };
        }
    }

}
