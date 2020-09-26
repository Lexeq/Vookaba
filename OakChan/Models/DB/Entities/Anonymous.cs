using System;

namespace OakChan.Models.DB.Entities
{
    public class Anonymous
    {
        public Guid Id { get; set; }

        public string IP { get; set; }

        public DateTime Created { get; set; }
    }
}
