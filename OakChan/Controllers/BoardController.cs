using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OakChan.Models;
using OakChan.Models.DB.Entities;
using OakChan.Models.Interfces;

namespace OakChan.Controllers
{
    public class BoardController : Controller
    {
        private const int threadsPerPage = 10;
        private readonly IBoardService boardService;

        public BoardController(IBoardService boardService)
        {
            this.boardService = boardService;
        }

        public async Task<IActionResult> Index(string board)
            => View(await boardService.GetBoardPreviewAsync(board, 1, threadsPerPage));

        [HttpPost]
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
