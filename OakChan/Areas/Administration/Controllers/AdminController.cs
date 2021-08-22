﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OakChan.Areas.Administration.ViewModels;
using OakChan.Common;
using OakChan.Controllers.Base;
using OakChan.Services;
using OakChan.Services.DTO;
using System;
using System.Threading.Tasks;

namespace OakChan.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : OakController
    {
        private readonly IBoardService boardService;
        private readonly IMapper mapper;
        private readonly ILogger<AdminController> logger;

        public AdminController(IBoardService boardService,
                               IMapper mapper,
                               ILogger<AdminController> logger)
        {
            this.boardService = boardService;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var dash = new AdminDashboardViemModel()
            {
                Boards = await boardService.GetBoardsAsync(true)
            };

            return View(dash);
        }

        [HttpGet]
        public IActionResult CreateBoard()
        {
            return View(new BoardPropertiesViewModel
            {
                BumpLimit = OakConstants.BoardConstants.DefaultBumpLimit
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoard(BoardPropertiesViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var dto = mapper.Map<BoardDto>(vm);
                try
                {
                    await boardService.CreateBoardAsync(dto);
                    logger.LogInformation($"Board '{dto.Key}' created by {User.Identity.Name}.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Cant't create the board '{dto.Key}'.");
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(vm);
                }
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBoard(string boardKey)
        {
            if (string.IsNullOrEmpty(boardKey))
            {
                return BadRequest();
            }
            try
            {
                await boardService.DeleteBoardAsync(boardKey);
                logger.LogInformation($"Board '{boardKey}' deleted by {User.Identity.Name}.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Cant't delete the board '{boardKey}'.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return base.Error(500, "Cant't delete the board. See logs for details.", ex.Message);
            }
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> EditBoard(string boardKey)
        {
            if (string.IsNullOrWhiteSpace(boardKey))
            {
                return BadRequest();
            }
            var board = await boardService.GetBoardInfoAsync(boardKey);
            if (board == null)
            {
                return Error(404, $"Board '{boardKey}' not found.");
            }

            var upd = mapper.Map<BoardPropertiesViewModel>(board);
            return View(upd);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBoard(BoardPropertiesViewModel board)
        {
            if (ModelState.IsValid)
            {
                var dto = mapper.Map<BoardDto>(board);
                try
                {
                    await boardService.UpdateBoardAsync(dto.Key, dto);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View();
                }
                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                return View(board);
            }
        }
    }
}
