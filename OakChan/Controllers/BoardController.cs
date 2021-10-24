using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.Common;
using OakChan.Controllers.Base;
using OakChan.Services;
using OakChan.Services.DTO;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class BoardController : OakController
    {
        private readonly IBoardService boardService;
        private readonly IStringLocalizer<BoardController> localizer;
        private readonly IMapper mapper;
        private readonly IModLogService modLogs;
        private readonly ILogger<BoardController> logger;

        public BoardController(
            IBoardService boardService,
            IStringLocalizer<BoardController> localizer,
            IMapper mapper,
            IModLogService modLogs,
            ILogger<BoardController> logger)
        {
            this.boardService = boardService;
            this.localizer = localizer;
            this.mapper = mapper;
            this.modLogs = modLogs;
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
                boardInfo.IsDisabled && !User.IsInRole(OakConstants.Roles.Administrator))
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
                Threads = mapper.Map<IEnumerable<ThreadPreviewViewModel>>(threads),
                PagesInfo = new PaginatorViewModel
                {
                    PageNumber = page,
                    TotalPages = pagesCount
                }
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Policy = OakConstants.Policies.CanEditBoards)]
        [Route("board/create", Name = "createBoard")]
        public IActionResult Create()
        {
            return View(new BoardPropertiesViewModel
            {
                BumpLimit = OakConstants.BoardConstants.DefaultBumpLimit
            });
        }

        [HttpPost]
        [Authorize(Policy = OakConstants.Policies.CanEditBoards)]
        [Route("board/create", Name = "createBoard")]
        public async Task<IActionResult> Create(BoardPropertiesViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var dto = mapper.Map<BoardDto>(vm);
                try
                {
                    await boardService.CreateBoardAsync(dto);
                    await modLogs.LogAsync(ApplicationEvent.BoardCreate, vm.BoardKey);
                    logger.LogInformation($"Board '{dto.Key}' created by {User.Identity.Name}.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Cant't create the board '{dto.Key}'.");
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(vm);
                }
                return RedirectToRoute("board", new { Board = dto.Key });
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        [Authorize(Policy = OakConstants.Policies.CanEditBoards)]
        public async Task<IActionResult> Edit([FromRoute(Name = "board")] string boardKey)
        {
            if (string.IsNullOrWhiteSpace(boardKey))
            {
                return BadRequest();
            }
            var board = await boardService.GetBoardInfoAsync(boardKey);
            if (board == null)
            {
                return Error(404, $"Board '{boardKey}' not found.");
            }

            var upd = mapper.Map<BoardPropertiesViewModel>(board);
            return View(upd);
        }

        [HttpPost]
        [Authorize(Policy = OakConstants.Policies.CanEditBoards)]
        public async Task<IActionResult> Update(BoardPropertiesViewModel boardProps, string board)
        {
            if (ModelState.IsValid)
            {
                var dto = mapper.Map<BoardDto>(boardProps);
                try
                {
                    await boardService.UpdateBoardAsync(board, dto);
                    await modLogs.LogAsync(ApplicationEvent.BoardEdit, board);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View();
                }
                return RedirectToRoute("board", new { Board = boardProps.BoardKey });
            }
            else
            {
                return View(board);
            }
        }

        [HttpPost]
        [Authorize(Policy = OakConstants.Policies.CanEditBoards)]
        public async Task<IActionResult> Delete(string board)
        {
            if (string.IsNullOrEmpty(board))
            {
                return BadRequest();
            }
            try
            {
                await boardService.DeleteBoardAsync(board);
                await modLogs.LogAsync(ApplicationEvent.BoardDelete, board);
                logger.LogInformation($"Board '{board}' deleted by {User.Identity.Name}.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Cant't delete the board '{board}'.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return base.Error(500, "Cant't delete the board. See logs for details.", ex.Message);
            }
            return RedirectToRoute(new { Area = "Administration", Controller = "Admin", Action = "Dashboard" });
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
