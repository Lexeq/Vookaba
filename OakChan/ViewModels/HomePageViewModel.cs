using OakChan.DAL.Entities;
using OakChan.Models;
using System.Collections.Generic;

namespace OakChan.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<Board> Boards { get; set; }

        public IEnumerable<TopThredInfo> TopThreads { get; set; }
    }
}
