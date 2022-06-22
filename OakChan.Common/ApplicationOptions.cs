using System;

namespace OakChan.Common
{
    public class ApplicationOptions
    {
        public bool RegistrationByInvitation { get; private set; } = false;

        public bool IsAnonymousPostingAllowed { get; private set; } = true;

        public bool DoubleCheckPermissions { get; private set; } = true;

        public TimeSpan PermissionsCheckInterval { get; private set; } = TimeSpan.FromDays(1);
    }
}
