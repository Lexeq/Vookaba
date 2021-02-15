using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.Deanon;
using OakChan.Mapping;
using OakChan.Services;
using OakChan.Services.DTO;
using OakChan.Utils;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class ThreadController : Controller
    {
        private readonly IThreadService threads;
        private readonly IStringLocalizer<ThreadController> localizer;
        private readonly ILogger<ThreadController> logger;
        private readonly IMapper mapper;

        public ThreadController(IThreadService threads,
            IStringLocalizer<ThreadController> localizer,
            ILogger<ThreadController> logger,
            IMapper mapper)
        {
            this.threads = threads;
            this.localizer = localizer;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(string board, int thread)
        {
            var threadDto = await threads.GetThreadAsync(board, thread);

            if (threadDto == null || (threadDto.Board.IsDisabled && !User.IsInRole(OakConstants.DefaultAdministratorRole)))
            {
                return View("Error", new ErrorViewModel
                {
                    Code = 404,
                    Title = localizer["Not found"],
                    Description = localizer["Thread not found."]
                });
            }

            var vm = mapper.Map<ThreadViewModel>(threadDto.Thread);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Policy = DeanonDefaults.DeanonPolicy)]
        public async Task<IActionResult> CreatePostAsync(string board, int thread, PostFormViewModel postFormVM)
        {
            var anonId = await HttpContext.GetAnonGuidAsync();
            if (ModelState.IsValid)
            {
                var postCreationDto = mapper.Map<PostCreationDto>(postFormVM, opt => opt.Items[StringConstants.UserId] = anonId);
                try
                {
                    var boardThread = await threads.GetThreadAsync(board, thread);
                    if (boardThread.Board.IsDisabled && !User.IsInRole(OakConstants.DefaultAdministratorRole))
                    {
                        return NotFound();
                    }
                    var newPost = await threads.AddPostToThreadAsync(board, thread, postCreationDto);
                    return RedirectToRoute("thread", new { Board = board, Thread = thread }, $"p{newPost.PostId}");
                }
                catch (KeyNotFoundException ex)
                {
                    logger.LogWarning($"Bad request. From {anonId} to {nameof(CreatePostAsync)}. {ex.Message}");
                    return BadRequest();
                }
            }
            else
            {
                logger.LogWarning($"Invalid model state from {anonId} to {nameof(CreatePostAsync)}. " +
                      string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));
                return BadRequest();
            }
        }
    }
}
