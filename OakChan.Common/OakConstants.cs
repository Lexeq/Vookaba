namespace OakChan.Common
{
    public static class OakConstants
    {
        public const int MinPasswordLength = 8;
        public const string AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+абвгдеёжзийклмнопрстуфхцчшщъыьэюяФБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        public const string DefaultAdministratorRole = "Administrator";

        public static class PostConstants
        {
            public const int NameMaxLength = 32;
            public const int MessageMaxLength = 4096;
            public const int MaxFileSize = 1536 * 1024;
        }

        public static class ThreadConstants
        {
            public const int SubjectMaxLength = 32;
            public const int MaxFileSize = 2048 * 1024;
        }

        public static class BoardConstants
        {
            public const int DefaultBumpLimit = 250;
            public const int MaxKeyLength = 10;
            public const int MaxNameLength = 64;
        }
    }
}
