using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.Deanon;
using OakChan.Mapping;
using OakChan.Services;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class BoardController : Controller
    {
        private const int threadsPerPage = 10;
        private readonly IBoardService boardService;
        private readonly IStringLocalizer<BoardController> localizer;
        private readonly IMapper mapper;
        private readonly ILogger<BoardController> logger;

        public BoardController(
            IBoardService boardService,
            IStringLocalizer<BoardController> localizer,
            IMapper mapper,
            ILogger<BoardController> logger)
        {
            this.boardService = boardService;
            this.localizer = localizer;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(string board, int page = 1)
        {
            var boardInfo = await boardService.GetBoardAsync(board);
            if (boardInfo == null)
            {
                return BoardDoesNotExist(board);
            }

            var pageDto = await boardService.GetBoardPageAsync(board, Math.Max(page, 1), threadsPerPage);
            if (pageDto.PageNumber > 1 && pageDto.Threads.Count == 0)
            {
                return PageNotFound(board, page);
            }

            var vm = mapper.Map<BoardPageViewModel>(pageDto, opt =>
            {
                opt.Items[StringConstants.BoardName] = board;
                opt.Items[StringConstants.PagesCount] = (int)Math.Ceiling((double)boardInfo.ThreadsCount / threadsPerPage);
            });

            return View(vm);
        }

        [HttpPost]
        [Authorize(Policy = DeanonDefaults.DeanonPolicy)]
        public async Task<IActionResult> CreateThreadAsync(OpPostFormViewModel opPost)
        {
            if (ModelState.IsValid)
            {
                var anonId = await HttpContext.GetAnonGuidAsync();

                var postData = await opPost.ToPostCreationData(anonId);

                var t = await boardService.CreateThreadAsync(opPost.Board, postData);

                return RedirectToRoute("thread", new { Board = t.BoardId, Thread = t.Id });
            }
            else
            {
                logger.LogWarning("Bad request. " +
                    string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));

                return (opPost == null || opPost.Board == null) ? BadRequest() : (IActionResult)RedirectToRoute("board", new { opPost.Board });
            }
        }

        private ViewResult BoardDoesNotExist(string board)
        {
            return this.ErrorView(new ErrorViewModel
            {
                Code = 404,
                Title = localizer["Not found"],
                Description = localizer["Board {0} does not exist.", board]
            });
        }

        private IActionResult PageNotFound(string board, int page)
        {
            return this.ErrorView(new ErrorViewModel
            {
                Code = 404,
                Title = localizer["Not found"],
                Description = localizer["Page {0} does not exist on board /{1}/.", page, board]
            });
        }
    }
}
