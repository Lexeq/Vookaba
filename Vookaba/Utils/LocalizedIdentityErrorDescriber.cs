using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Vookaba.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vookaba.Utils
{
    public class LocalizedIdentityErrorDescriber : ExtendedIdentityErrorDescriber
    {
        private readonly IStringLocalizer<LocalizedIdentityErrorDescriber> localizer;

        public LocalizedIdentityErrorDescriber(IStringLocalizer<LocalizedIdentityErrorDescriber> localizer)
        {
            this.localizer = localizer;
        }

        public override IdentityError DefaultError()
            => GetErrorByCode(nameof(DefaultError));

        public override IdentityError ConcurrencyFailure()
            => GetErrorByCode(nameof(ConcurrencyFailure));

        public override IdentityError PasswordMismatch()
            => GetErrorByCode(nameof(PasswordMismatch));

        public override IdentityError InvalidToken()
            => GetErrorByCode(nameof(InvalidToken));

        public override IdentityError LoginAlreadyAssociated()
            => GetErrorByCode(nameof(LoginAlreadyAssociated));

        public override IdentityError InvalidUserName(string userName)
            => GetErrorByCode(nameof(InvalidUserName), userName);

        public override IdentityError InvalidEmail(string email)
            => GetErrorByCode(nameof(InvalidEmail), email);

        public override IdentityError DuplicateUserName(string userName)
            => GetErrorByCode(nameof(DuplicateUserName), userName);

        public override IdentityError DuplicateEmail(string email)
            => GetErrorByCode(nameof(DuplicateEmail), email);

        public override IdentityError InvalidRoleName(string role)
            => GetErrorByCode(nameof(InvalidRoleName), role);

        public override IdentityError DuplicateRoleName(string role)
            => GetErrorByCode(nameof(DuplicateRoleName), role);

        public override IdentityError UserAlreadyHasPassword()
            => GetErrorByCode(nameof(UserAlreadyHasPassword));

        public override IdentityError UserLockoutNotEnabled()
            => GetErrorByCode(nameof(UserLockoutNotEnabled));

        public override IdentityError UserAlreadyInRole(string role)
            => GetErrorByCode(nameof(UserAlreadyInRole), role);

        public override IdentityError UserNotInRole(string role)
            => GetErrorByCode(nameof(UserNotInRole), role);

        public override IdentityError PasswordTooShort(int length)
            => GetErrorByCode(nameof(PasswordTooShort), length);

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => GetErrorByCode(nameof(PasswordRequiresNonAlphanumeric));

        public override IdentityError PasswordRequiresDigit()
            => GetErrorByCode(nameof(PasswordRequiresDigit));

        public override IdentityError PasswordRequiresLower()
            => GetErrorByCode(nameof(PasswordRequiresLower));

        public override IdentityError PasswordRequiresUpper()
            => GetErrorByCode(nameof(PasswordRequiresUpper));

        public override IdentityError RecoveryCodeRedemptionFailed()
            => GetErrorByCode(nameof(RecoveryCodeRedemptionFailed));

        public override IdentityError NoInvitation()
            => GetErrorByCode(nameof(NoInvitation));

        public override IdentityError InvitationExpired(DateTime expired)
            => GetErrorByCode(nameof(InvitationExpired), expired);

        public override IdentityError InvitationUsed()
            => GetErrorByCode(nameof(InvitationUsed));

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => GetErrorByCode(nameof(PasswordRequiresUniqueChars), uniqueChars);

        private IdentityError GetErrorByCode(string code)
        {
            return new IdentityError()
            {
                Code = code,
                Description = localizer[code]
            };
        }

        private IdentityError GetErrorByCode(string code, params object[] parameters)
        {
            return new IdentityError
            {
                Code = code,
                Description = localizer[code, parameters]
            };
        }
    }
}
