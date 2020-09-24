using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OakChan.Deanon;
using OakChan.Models;
using OakChan.Models.DB.Entities;
using OakChan.Models.Interfces;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class BoardController : Controller
    {
        private const int threadsPerPage = 10;
        private readonly IBoardService boardService;
        private readonly IStringLocalizer<BoardController> localizer;

        public BoardController(IBoardService boardService, IStringLocalizer<BoardController> localizer)
        {
            this.boardService = boardService;
            this.localizer = localizer;
        }

        public async Task<IActionResult> Index(string board)
        {
            var b = await boardService.GetBoardPreviewAsync(board, 1, threadsPerPage);
            if (b == null)
            {
                return View("Error", new ErrorViewModel {
                    Code = 404,
                    Title =  localizer["Not found"], 
                    Description = localizer["Board {0} does not exist.", board]});
            }
            return View(b);
        }

        [HttpPost]
        [Authorize(Policy = DeanonDefaults.DeanonPolicy)]
        public async Task<IActionResult> CreateThreadAsync(string board, string subject, string text, string name, IFormFile atachedImage)
        {
            var t = await boardService.CreateThreadAsync(board, new PostCreationData()
            {
                Name = name,
                Subject = subject,
                Text = text
            });

            return RedirectToRoute("thread", new { Board = t.BoardId, Thread = t.Id });
        }
    }
}
