using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OakChan.Services;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBoardService boardService;
        private readonly ITopThreadsService topThreads;
        private readonly IMapper mapper;

        public HomeController(IBoardService boardService, ITopThreadsService threads, IMapper mapper)
        {
            this.boardService = boardService;
            this.topThreads = threads;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var boards = (await boardService.GetBoardsAsync(false)).ToList();

            var vm = new HomePageViewModel
            {
                Boards = boards,
                LastCreatedThreads = mapper.Map<List<ThreadPreviewViewModel>>(await topThreads.GetTopThreadsByCreationTimeAsync(3)),
                LastUpdatedThreads = mapper.Map<List<ThreadPreviewViewModel>>(await topThreads.GetTopThreadsByLastPostAsync(3)),
            };

            return View(vm);
        }
    }
}
