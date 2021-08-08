using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.Common;
using OakChan.Controllers.Base;
using OakChan.Deanon;
using OakChan.Identity;
using OakChan.Mapping;
using OakChan.Services;
using OakChan.Services.DTO;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class BoardController : OakController
    {
        private readonly IBoardService boardService;
        private readonly IThreadService threadService;
        private readonly IStringLocalizer<BoardController> localizer;
        private readonly IMapper mapper;
        private readonly ILogger<BoardController> logger;

        public BoardController(
            IBoardService boardService,
            IThreadService threadService,
            IStringLocalizer<BoardController> localizer,
            IMapper mapper,
            ILogger<BoardController> logger)
        {
            this.boardService = boardService;
            this.threadService = threadService;
            this.localizer = localizer;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(string board, int page = 1, int pageSize = 10)
        {
            if (page < 1)
            {
                return PageNotFound(board, page);
            }
            
            BoardInfoDto boardInfo = await boardService.GetBoardInfoAsync(board);

            if (boardInfo == null ||
                boardInfo.IsDisabled && !User.IsInRole(OakConstants.DefaultAdministratorRole))
            {
                return BoardDoesNotExist(board);
            }

            pageSize = CoercePageSize(pageSize);
            var offset = (page - 1) * pageSize;
            var pagesCount = Math.Max(1, (int)Math.Ceiling((double)boardInfo.ThreadsCount / pageSize));

            if (offset >= boardInfo.ThreadsCount && page != 1)
            {
                return PageNotFound(board, page);
            }

            var threads = await boardService.GetThreadPreviewsAsync(boardInfo.Key, offset, pageSize, 2);

            var vm = new BoardPageViewModel
            {
                Key = board,
                Name = boardInfo.Name,
                PageNumber = page,
                Threads = mapper.Map<IEnumerable<ThreadPreviewViewModel>>(threads),
                TotalPages = pagesCount
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Policy = DeanonConstants.DeanonPolicy)]
        public async Task<IActionResult> CreateThreadAsync(string board, ThreadFormViewModel opPost)
        {
            var userInfo = HttpContext.Features.Get<IDeanonFeature>();

            if (ModelState.IsValid)
            {
                var threadData = mapper.Map<ThreadCreationDto>(opPost, opt =>
                {
                    opt.Items[StringConstants.UserInfo] = userInfo;
                });

                var boardInfo = await boardService.GetBoardInfoAsync(board);
                if (boardInfo == null || boardInfo.IsDisabled)
                {
                    logger.LogWarning($"Bad request. From {userInfo.UserToken} to {nameof(CreateThreadAsync)}. Bad board key {board}");

                    return BadRequest();
                }
                var t = await threadService.CreateThreadAsync(boardInfo.Key, threadData);
                return RedirectToRoute("thread", new { Board = boardInfo.Key, Thread = t.ThreadId });
            }
            else
            {
                logger.LogWarning($"Invalid model state from {userInfo.UserToken} to {nameof(CreateThreadAsync)}. " +
                      string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));
                return BadRequest();
            }
        }

        private IActionResult BoardDoesNotExist(string board)
        {
            return Error(404, localizer["Not found"], localizer["Board {0} does not exist.", board]);
        }

        private IActionResult PageNotFound(string board, int page)
        {
            return Error(404, localizer["Not found"], localizer["Page {0} does not exist on board /{1}/.", page, board]);
        }

        private int CoercePageSize(int pageSize)
        {
            const int minValue = 5;
            const int maxValue = 50;
            return Math.Min(Math.Max(minValue, pageSize), maxValue);
        }
    }
}
