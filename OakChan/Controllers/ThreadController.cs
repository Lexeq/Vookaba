using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.Deanon;
using OakChan.Services;
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
            if (threadDto == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Code = 404,
                    Title = localizer["Not found"],
                    Description = localizer["Thread not found."]
                });
            }

            var vm = mapper.Map<ThreadViewModel>(threadDto);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Policy = DeanonDefaults.DeanonPolicy)]
        public async Task<IActionResult> CreatePost(PostFormViewModel post)
        {
            var anonId = await HttpContext.GetAnonGuidAsync();
            if (ModelState.IsValid)
            {
                await threads.CreatePostAsync(post.Board, post.Thread.Value, await post.ToPostCreationData(anonId));
            }
            else
            {
                logger.LogWarning("Bad request. " +
                    string.Join(Environment.NewLine, ModelState.Root.Errors.Select(e => e.ErrorMessage)));
            }
            return (post == null || post.Board == null || post.Thread == null) ? BadRequest() : (IActionResult)RedirectToRoute("thread", new { post.Board, post.Thread }); ;
        }
    }
}
