using System;

namespace OakChan.DAL.Entities
{
    public class IdToken
    {
        public Guid Id { get; set; }

        public string IP { get; set; }

        public DateTime Created { get; set; }
    }
}
