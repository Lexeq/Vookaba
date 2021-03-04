using OakChan.Identity;
using System;

namespace OakChan.DAL.Entities
{
    public class IdToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        public int? UserId { get; set; }
    }
}
