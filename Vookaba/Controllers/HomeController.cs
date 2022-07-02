using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Vookaba.Services.Abstractions;
using Vookaba.ViewModels.Home;
using Vookaba.ViewModels.Thread;

namespace Vookaba.Controllers
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
                LastCreatedThreads = mapper.Map<List<ThreadPreviewViewModel>>(await topThreads.GetLastCreatedThreadsAsync(3)),
                LastUpdatedThreads = mapper.Map<List<ThreadPreviewViewModel>>(await topThreads.GetLastRepliedThreadsAsync(3)),
            };

            return View(vm);
        }
    }
}
