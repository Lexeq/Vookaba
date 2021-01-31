using System.Collections.Generic;

namespace OakChan.DAL.Entities
{
    public class Board
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public bool IsHidden { get; set; }

        public IEnumerable<Thread> Threads { get; set; }
    }
}
