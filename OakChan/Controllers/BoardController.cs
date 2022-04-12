﻿using System;
using System.Collections.Generic;
using System.Text.Json;
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
using BoardConsts = OakChan.Common.OakConstants.BoardConstants;

namespace OakChan.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class BoardController : OakController
    {
        private readonly IBoardService boardService;
        private readonly IPostService postService;
        private readonly IStringLocalizer<BoardController> localizer;
        private readonly IMapper mapper;
        private readonly IModLogService modLogs;
        private readonly ILogger<BoardController> logger;

        public BoardController(
            IBoardService boardService,
            IPostService postService,
            IStringLocalizer<BoardController> localizer,
            IMapper mapper,
            IModLogService modLogs,
            ILogger<BoardController> logger)
        {
            this.boardService = boardService;
            this.postService = postService;
            this.localizer = localizer;
            this.mapper = mapper;
            this.modLogs = modLogs;
            this.logger = logger;
        }

        public async Task<IActionResult> Index([FromRoute(Name = "board")] string boardKey, int page = 1)
        {
            if (page < 1)
            {
                return PageNotFound(boardKey, page);
            }

            var board = await boardService.GetBoardAsync(boardKey);

            if (board == null ||
                board.IsDisabled && !User.IsInRole(OakConstants.Roles.Administrator))
            {
                return BoardDoesNotExist(boardKey);
            }

            var offset = (page - 1) * BoardConsts.PageSize;
            var threads = await boardService.GetThreadPreviewsAsync(board.Key, offset, BoardConsts.PageSize);
            var pagesCount = Math.Max(1, (int)Math.Ceiling((double)threads.TotalCount / BoardConsts.PageSize));

            if (threads.CurrentItems?.Count == 0 && page != 1)
            {
                return PageNotFound(boardKey, page);
            }


            var vm = new BoardPageViewModel
            {
                Key = boardKey,
                Name = board.Name,
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
            var board = await boardService.GetBoardAsync(boardKey);
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

        [HttpPost]
        [Authorize(Policy = OakConstants.Policies.CanDeletePosts)]
        public async Task<IActionResult> BulkDeletePosts([FromRoute] string board, [FromBody] PostsDeletionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var post = await postService.GetByNumberAsync(board, vm.PostNumber);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            try
            {
                if (vm.Area == PostsDeletionViewModel.DeletingArea.Single)
                {
                    await postService.DeleteByIdAsync(post.PostId);
                    await modLogs.LogAsync(
                        ApplicationEvent.PostDelete,
                        post.PostId.ToString(),
                        JsonSerializer.Serialize(new { vm.Reason }));
                }
                else
                {
                    await postService.DeleteManyAsync(post.PostId, vm.Mode, (SearchArea)vm.Area);

                    await modLogs.LogAsync(
                        ApplicationEvent.PostBulkDelete,
                        post.PostId.ToString(),
                        JsonSerializer.Serialize(new
                        {
                            vm.Reason,
                            vm.Mode,
                            vm.Area,
                            AuthorIp = post.AuthorIP.ToString(),
                            post.AuthorId
                        }));
                }
                return Ok();
            }
            catch (Exception ex)
                when (ex is ArgumentException or ArgumentNullException)
            {
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Can't delete post.");
                throw;
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
    }
}
