using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OakChan.Models;
using OakChan.Models.Interfaces;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBoardService boardService;
        private readonly FavoriteThreadsService stat;

        public HomeController(IBoardService boardService, FavoriteThreadsService stat)
        {
            this.boardService = boardService;
            this.stat = stat;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new HomePageViewModel
            {
                Boards = await boardService.GetBoardsAsync(),
                TopThreads = stat.GetMostPopularPerBoard(3)
            };

            return View(vm);
        }
    }
}
