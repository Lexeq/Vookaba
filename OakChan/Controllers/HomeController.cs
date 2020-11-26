using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OakChan.Services;
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
                TopThreads = stat.GetMostPopularThreadOnBoard(3)
            };

            return View(vm);
        }
    }
}
