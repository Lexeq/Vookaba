using System;

namespace OakChan.Common
{
    public class ChanOptions
    {
        public bool PublicRegistrationEnabled { get; set; } = false;

        public bool DoubleCheckPermissions { get; set; } = true;

        public TimeSpan CheckPermissionsInterval { get; set; } = TimeSpan.FromDays(1);
    }
}
