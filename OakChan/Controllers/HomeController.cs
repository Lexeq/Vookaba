using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OakChan.Models;
using OakChan.Models.Interfaces;

namespace OakChan.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBoardService boardService;

        public HomeController(IBoardService boardService)
        {
            this.boardService = boardService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await boardService.GetBoardsAsync());
        }
    }
}
