using OakChan.Models;
using OakChan.Models.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<Board> Boards { get; set; }

        public IEnumerable<TopThredInfo> TopThreads { get; set; }
    }
}
