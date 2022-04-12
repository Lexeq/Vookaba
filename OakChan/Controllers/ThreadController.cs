﻿using System;
using System.Linq;
using System.Security.Claims;
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
    public class ThreadController : OakController
    {
        private readonly IThreadService threads;
        private readonly IBoardService boards;
        private readonly IStringLocalizer<ThreadController> localizer;
        private readonly ILogger<ThreadController> logger;
        private readonly IMapper mapper;

        public ThreadController(IThreadService threads,
            IBoardService boards,
            IStringLocalizer<ThreadController> localizer,
            IMapper mapper,
            ILogger<ThreadController> logger)
        {
            this.threads = threads;
            this.boards = boards;
            this.localizer = localizer;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(string board, int thread)
        {
            var showAll = User.IsInRole(OakConstants.Roles.Administrator);
            var boardDto = await boards.GetBoardAsync(board);
            if (boardDto?.IsDisabled == false || showAll)
            {
                var threadDto = await threads.GetThreadAsync(boardDto.Key, thread);
                if (threadDto != null)
                {
                    var vm = mapper.Map<ThreadViewModel>(threadDto);
                    return View(vm);
                }
            }

            return Error(404, localizer["Not found"], localizer["Thread not found."]);
        }

        [HttpPost]
        [Authorize(Policy = OakConstants.Policies.CanPost)]
        [Route("{board:alpha}/createThread", Name = "createThread")]
        public async Task<IActionResult> CreateThreadAsync(string board, ThreadFormViewModel opPost)
        {
            if (ModelState.IsValid)
            {
                var threadData = mapper.Map<ThreadCreationDto>(opPost);

                var boardInfo = await boards.GetBoardAsync(board);
                if (boardInfo == null || boardInfo.IsDisabled)
                {
                    logger.LogWarning($"Bad request. From {User.FindFirst(OakConstants.ClaimTypes.AuthorToken)} to {nameof(CreateThreadAsync)}. Bad board key {board}");

                    return BadRequest();
                }
                var t = await threads.CreateThreadAsync(boardInfo.Key, threadData);
                return RedirectToRoute("thread", new { Board = boardInfo.Key, Thread = t.ThreadId });
            }
            else
            {
                logger.LogWarning($"Invalid model state from {OakConstants.ClaimTypes.AuthorToken} to {nameof(CreateThreadAsync)}. " +
                      string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Policy = OakConstants.Policies.CanPost)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePostAsync(string board, int thread, PostFormViewModel postFormVM)
        {
            if (ModelState.IsValid)
            {
                var boardDto = await boards.GetBoardAsync(board);
                if (boardDto?.IsDisabled == false)
                {
                    var threadDto = await threads.GetThreadInfoAsync(boardDto.Key, thread);
                    if (threadDto != null && !threadDto.IsReadOnly)
                    {
                        var postCreationDto = mapper.Map<PostCreationDto>(postFormVM);
                        postCreationDto.OpMark = postCreationDto.OpMark && threadDto.Author == Guid.Parse(User.FindFirstValue(Common.OakConstants.ClaimTypes.AuthorToken));
                        var newPost = await threads.AddPostToThreadAsync(thread, postCreationDto);
                        return RedirectToRoute("thread", new { Board = board, Thread = thread }, $"p{newPost.PostId}");
                    }
                }
                logger.LogWarning($"Invalid arguments from {User.FindFirst(OakConstants.ClaimTypes.AuthorToken)}({IP}) to {nameof(CreatePostAsync)}. " +
                    string.Join(Environment.NewLine, $"board: {board}", $"thread: {thread}."));
                return BadRequest();
            }
            else
            {
                logger.LogWarning($"Invalid model state from {User.FindFirst(OakConstants.ClaimTypes.AuthorToken)}({IP}) to {nameof(CreatePostAsync)}. " +
                      string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(OakConstants.Policies.CanEditThread)]
        public async Task<IActionResult> PinThread(string board, [FromRoute(Name = "thread")] int threadId, bool pin)
        {
            var thread = await threads.GetThreadInfoAsync(board, threadId);
            if (thread == null) { return BadRequest(); }
            await threads.SetIsPinned(threadId, pin);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(OakConstants.Policies.CanEditThread)]
        public async Task<IActionResult> LockThread(string board, [FromRoute(Name = "thread")] int threadId, bool @lock)
        {
            var thread = await threads.GetThreadInfoAsync(board, threadId);
            if (thread == null) { return BadRequest(); }
            await threads.SetIsReadOnly(threadId, @lock);
            return Ok();
        }
    }
}
