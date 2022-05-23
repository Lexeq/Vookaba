using System;
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
            var isAdmin = UserRole == OakConstants.Roles.Administrator;
            var boardDto = await boards.GetBoardAsync(board);
            if (boardDto?.IsDisabled == false || isAdmin)
            {
                var threadDto = await threads.GetThreadAsync(boardDto.Key, thread);
                if (threadDto != null)
                {
                    var vm = mapper.Map<ThreadViewModel>(threadDto);
                    vm.IsReadOnly |= boardDto.IsReadOnly;
                    return View(vm);
                }
            }

            return Error(404, localizer["Not found"], localizer["Thread not found."]);
        }

        [HttpPost]
        [Authorize(Policy = OakConstants.Policies.CanPost)]
        [Route("{board:alpha}/createThread", Name = "createThread")]
        public async Task<IActionResult> Create(string board, ThreadFormViewModel opPost)
        {
            if (ModelState.IsValid)
            {
                var boardInfo = await boards.GetBoardAsync(board);
                if (boardInfo == null || boardInfo.IsDisabled || boardInfo.IsReadOnly)
                {
                    logger.LogWarning($"Bad request. From {User.FindFirst(OakConstants.ClaimTypes.AuthorToken)} to {nameof(Create)}. Bad board key {board}");
                    return BadRequest();
                }
                var threadData = mapper.Map<ThreadCreationDto>(opPost);

                var t = await threads.CreateThreadAsync(boardInfo.Key, threadData);
                return RedirectToRoute("thread", new { Board = boardInfo.Key, Thread = t.ThreadId });
            }
            else
            {
                logger.LogWarning($"Invalid model state from {OakConstants.ClaimTypes.AuthorToken} to {nameof(Create)}. " +
                      string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Policy = OakConstants.Policies.CanPost)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPost(string board, int thread, PostFormViewModel postFormVM)
        {
            if (ModelState.IsValid)
            {
                var boardDto = await boards.GetBoardAsync(board);
                if (boardDto != null && !boardDto.IsDisabled && !boardDto.IsReadOnly)
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
                logger.LogWarning($"Invalid arguments from {User.FindFirst(OakConstants.ClaimTypes.AuthorToken)}({IP}) to {nameof(AddPost)}. " +
                    string.Join(Environment.NewLine, $"board: {board}", $"thread: {thread}."));
                return BadRequest();
            }
            else
            {
                logger.LogWarning($"Invalid model state from {User.FindFirst(OakConstants.ClaimTypes.AuthorToken)}({IP}) to {nameof(AddPost)}. " +
                      string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));
                return BadRequest();
            }
        }
    }
}
