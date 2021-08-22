using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.Common;
using OakChan.Controllers.Base;
using OakChan.Deanon;
using OakChan.Mapping;
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
            var showAll = User.IsInRole(OakConstants.DefaultAdministratorRole);
            var boardDto = await boards.GetBoardInfoAsync(board);
            if (boardDto?.IsDisabled == false || showAll)
            {
                var threadDto = await threads.GetThreadAsync(boardDto.Key, thread);
                if (threadDto != null)
                {
                    var vm = mapper.Map<ThreadViewModel>(new ThreadBoardAggregationDto { Thread = threadDto, Board = boardDto });
                    return View(vm);
                }
            }

            return Error(404, localizer["Not found"], localizer["Thread not found."]);
        }

        [HttpPost]
        [Authorize(Policy = DeanonConstants.DeanonPolicy)]
        public async Task<IActionResult> CreatePostAsync(string board, int thread, PostFormViewModel postFormVM)
        {
            var userInfo = HttpContext.Features.Get<IDeanonFeature>();

            if (ModelState.IsValid)
            {
                var boardDto = await boards.GetBoardInfoAsync(board);
                if (boardDto?.IsDisabled == false)
                {
                    var threadDto = await threads.GetThreadAsync(boardDto.Key, thread);
                    if (threadDto != null && !threadDto.IsReadOnly)
                    {
                        var postCreationDto = mapper.Map<PostCreationDto>(postFormVM, opts => opts.Items[StringConstants.UserInfo] = userInfo);
                        var newPost = await threads.AddPostToThreadAsync(boardDto.Key, thread, postCreationDto);
                        return RedirectToRoute("thread", new { Board = board, Thread = thread }, $"p{newPost.PostId}");
                    }
                }
                logger.LogWarning($"Invalid arguments from {userInfo.UserToken}({userInfo.IPAddress}) to {nameof(CreatePostAsync)}. " +
                    string.Join(Environment.NewLine, $"board: {board}", $"thread: {thread}."));
                return BadRequest();
            }
            else
            {
                logger.LogWarning($"Invalid model state from {userInfo.UserToken}({userInfo.IPAddress}) to {nameof(CreatePostAsync)}. " +
                      string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));
                return BadRequest();
            }
        }
    }
}
