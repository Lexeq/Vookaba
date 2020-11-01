using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.Deanon;
using OakChan.Models;
using OakChan.Models.DB.Entities;
using OakChan.Models.Interfaces;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class BoardController : Controller
    {
        private const int threadsPerPage = 10;
        private readonly IBoardService boardService;
        private readonly IStringLocalizer<BoardController> localizer;
        private readonly ILogger<BoardController> logger;

        public BoardController(IBoardService boardService,
            IStringLocalizer<BoardController> localizer,
            ILogger<BoardController> logger)
        {
            this.boardService = boardService;
            this.localizer = localizer;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(string board, int page = 1)
        {
            if (page < 1)
            {
                return NotFound();
            }
            var b = await boardService.GetBoardPreviewAsync(board, (page - 1) * threadsPerPage, threadsPerPage);
            if (b == null)
            {
                return BoardNotExist(board);
            }
            if (page != 1 && b.Threads.Count() == 0)
            {
                return NotFound();
            }
            return View(new BoardViewModel
            {
                Board = b,
                OpPost = new OpPostViewModel { Board = b.Key },
                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)b.TotalThreadsCount / threadsPerPage)
            });
        }

        [HttpPost]
        [Authorize(Policy = DeanonDefaults.DeanonPolicy)]
        public async Task<IActionResult> CreateThreadAsync(OpPostViewModel opPost)
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

        private ViewResult BoardNotExist(string board) =>
            View("Error", new ErrorViewModel
            {
                Code = 404,
                Title = localizer["Not found"],
                Description = localizer["Board {0} does not exist.", board]
            });
    }
}
