namespace Vookaba.Common
{
    public static class ApplicationConstants
    {
        public const string Name = "Voochan";
        public const string Culture = "en";
        public const string UserOptionsFileName = "appsettings.edited.json";

        public static class Identity
        {
            public const int MinPasswordLength = 8;
            public const string AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+абвгдеёжзийклмнопрстуфхцчшщъыьэюяФБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            public const int CookieMaxAgeInDays = 15;
            public const int CookieExpireInDays = 30;
        }

        public static class PostConstants
        {
            public const int NameMaxLength = 32;
            public const int MessageMaxLength = 10000;
            public const int MaxFileSize = 1536 * 1024;
        }

        public static class ThreadConstants
        {
            public const int SubjectMaxLength = 42;
            public const int MaxFileSize = 2048 * 1024;
        }

        public static class BoardConstants
        {
            public const int DefaultBumpLimit = 250;
            public const int MaxKeyLength = 10;
            public const int MaxNameLength = 64;

            public const int PageSize = 10;
            public const int RecentRepliesShow = 3;
        }

        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Moderator = "Moderator";
            public const string Janitor = "Janitor";
            public const string NotInRole = "No role";
        }

        public static class ClaimTypes
        {
            public const string AuthorToken = "atkn";
            public const string BoardPermission = "brdPerm";
        }

        public static class Policies
        {
            public const string CanPost = "CanPost";
            public const string CanInviteUsers = "CanInvitePolicy";
            public const string CanEditBoards = "EditBoardPolicy";
            public const string CanDeletePosts = "DeletePostPolicy";
            public const string CanEditUsers = "EditUserPolicy";
            public const string HasStaffRole = "StaffRole";
            public const string HasBoardPermission = "BoardPermissionPolicy";
            public const string CanEditThreads = "ThreadEditPolicy";
        }
    }
}
