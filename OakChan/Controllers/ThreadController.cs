using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OakChan.Deanon;
using OakChan.Models;
using OakChan.Models.Interfaces;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class ThreadController : Controller
    {
        private readonly IThreadService threads;
        private readonly IStringLocalizer<ThreadController> localizer;

        public ThreadController(IThreadService threads, IStringLocalizer<ThreadController> localizer)
        {
            this.threads = threads;
            this.localizer = localizer;
        }

        public async Task<IActionResult> Index(string board, int thread)
        {
            var t = await threads.GetThreadAsync(board, thread);
            if (t == null)
            {
                return View("Error", new ErrorViewModel
                {
                    Code = 404,
                    Title = localizer["Not found"],
                    Description = localizer["Thread not found."]
                });
            }
            return View(new ThreadViewModel
            {
                Thread = t
            });
        }

        [HttpPost]
        [Authorize(Policy = DeanonDefaults.DeanonPolicy)]
        public async Task<IActionResult> CreatePost(PostViewModel post)
        {
            var anonId = await HttpContext.GetAnonGuidAsync();
            if (ModelState.IsValid)
            {
                await threads.CreatePostAsync(post.Board, post.Thread.Value, await post.ToPostCreationData(anonId));
            }

            return (post.Board == null || post.Thread == null) ? RedirectToRoute("default") : RedirectToRoute("thread", new { post.Board, post.Thread }); ;
        }
    }
}
