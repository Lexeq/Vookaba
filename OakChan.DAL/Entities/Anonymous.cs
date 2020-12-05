using System;

namespace OakChan.DAL.Entities
{
    public class Anonymous
    {
        public Guid Id { get; set; }

        public string IP { get; set; }

        public DateTime Created { get; set; }
    }
}
