using OakChan.Models.Interfces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Models.DB.Entities
{
    public class Board
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public IEnumerable<Thread> Threads { get; set; }
    }
}
