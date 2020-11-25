using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.Deanon;
using OakChan.Services;
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
            var b = await boardService.GetBoardPreviewAsync(board, (Math.Max(0, page - 1)) * threadsPerPage, threadsPerPage);
            if (b == null)
            {
                return BoardDoesNotExist(board);
            }
            if (page < 1 || (page != 1 && b.Threads.Count() == 0))
            {
                return PageNotFound(board, page);
            }

            return View(new BoardViewModel
            {
                Key = b.Key,
                Name = b.Name,
                ThreadsOnPage = b.Threads.Select(t => new ThreadPreviewViewModel
                {
                    ThreadId = t.Id,
                    PostsCount = t.TotalPostsCount,
                    PostsWithImageCount = t.PostsWithImageCount,
                    OpPost = PostViewModel.CreatePostViewModel(t.OpPost,board),
                    RecentPosts = t.RecentPosts?.Select(x => PostViewModel.CreatePostViewModel(x, board))
                }),
                TotalThreadsCount = b.TotalThreadsCount,
                OpPost = new OpPostFormViewModel { Board = b.Key },
                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)b.TotalThreadsCount / threadsPerPage)
            });
        }

        [HttpPost]
        [Authorize(Policy = DeanonDefaults.DeanonPolicy)]
        public async Task<IActionResult> CreateThreadAsync(OpPostFormViewModel opPost)
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

        private ViewResult BoardDoesNotExist(string board)
        {
            return this.ErrorView(new ErrorViewModel
            {
                Code = 404,
                Title = localizer["Not found"],
                Description = localizer["Board {0} does not exist.", board]
            });
        }

        private IActionResult PageNotFound(string board, int page)
        {
            return this.ErrorView(new ErrorViewModel
            {
                Code = 404,
                Title = localizer["Not found"],
                Description = localizer["Page {0} does not exist on board /{1}/.", page, board]
            });
        }
    }
}
