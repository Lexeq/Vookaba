﻿namespace Vookaba.Services.DTO
{
    public class ThreadInfoDto
    {
        public Guid Author { get; set; }

        public string BoardKey { get; set; } = null!;

        public int ThreadId { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsPinned { get; set; }

        public string? Subject { get; set; }

        public DateTime LastBump { get; set; }
    }
}
