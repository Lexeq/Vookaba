using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OakChan.Areas.Administration.ViewModels;
using OakChan.Common;
using OakChan.Controllers.Base;
using OakChan.Identity;
using OakChan.Services;
using OakChan.Services.DTO;
using OakChan.Utils;
using OakChan.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OakChan.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = OakConstants.Roles.Administrator)]
    [AutoValidateAntiforgeryToken]
    public class AdminController : OakController
    {
        private readonly IBoardService boardService;
        private readonly IMapper mapper;
        private readonly ILogger<AdminController> logger;
        private readonly ApplicationUserManager userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IModLogService modLogs;
        private readonly HttpStatusCodeDescriber statusCodeDescriber;

        public AdminController(IBoardService boardService,
                               IMapper mapper,
                               ILogger<AdminController> logger,
                               HttpStatusCodeDescriber statusCodeDescriber,
                               ApplicationUserManager users,
                               RoleManager<ApplicationRole> roles,
                               IModLogService modLogs)
        {
            this.boardService = boardService;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = users;
            this.roleManager = roles;
            this.modLogs = modLogs;
            this.statusCodeDescriber = statusCodeDescriber;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var dash = new AdminDashboardViemModel()
            {
                Boards = await boardService.GetBoardsAsync(true),
                Staff = new StuffViewModel
                {
                    Moderators = await userManager.GetUsersInRoleAsync(OakConstants.Roles.Moderator),
                    Janitors = await userManager.GetUsersInRoleAsync(OakConstants.Roles.Janitor)
                }
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
                    await modLogs.LogAsync(ApplicationEvent.BoardCreate, vm.BoardKey);
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
                await modLogs.LogAsync(ApplicationEvent.BoardDelete, boardKey);
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
        public async Task<IActionResult> UpdateBoard(BoardPropertiesViewModel board, string editingBoard)
        {
            if (ModelState.IsValid)
            {
                var dto = mapper.Map<BoardDto>(board);
                try
                {
                    await boardService.UpdateBoardAsync(editingBoard, dto);
                    await modLogs.LogAsync(ApplicationEvent.BoardCreate, editingBoard);
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

        [HttpGet]
        public async Task<IActionResult> EditStaff(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("No user id provided.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var boards = (await userManager.GetClaimsAsync(user))
                .Where(c => c.Type == OakConstants.ClaimTypes.BoardPermission)
                .Select(x => x.Value)
                .ToHashSet();

            var isAdmin = await userManager.IsInRoleAsync(user, OakConstants.Roles.Administrator);


            var vm = new EditStaffViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Boards = (await boardService.GetBoardsAsync(true))
                      .Select(x => new CheckableItem<string>(x.Key, isAdmin || boards.Contains(x.Key)))
                      .ToList(),
                Roles = isAdmin ? new[] { OakConstants.Roles.Administrator } : new[] { OakConstants.Roles.NotInRole, OakConstants.Roles.Moderator, OakConstants.Roles.Janitor },
                UserRole = isAdmin ? OakConstants.Roles.Administrator : (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? OakConstants.Roles.NotInRole
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStaff(EditStaffViewModel vm)
        {
            var user = await userManager.FindByIdAsync(vm.UserId);
            if (user == null)
            {
                return Error((int)HttpStatusCode.BadRequest, statusCodeDescriber.GetStatusCodeDescription((int)HttpStatusCode.BadRequest), "Incorrect user id.");
            }
            if (await userManager.IsInRoleAsync(user, OakConstants.Roles.Administrator))
            {
                return RedirectToAction(nameof(EditStaff), new { vm.UserId });
            }
            if (string.IsNullOrEmpty(vm.UserRole))
            {
                return Error((int)HttpStatusCode.BadRequest, statusCodeDescriber.GetStatusCodeDescription((int)HttpStatusCode.BadRequest), "Incorrect new user role.");
            }

            var roleResult = await UpdateUserRoleAsync(user, vm.UserRole);
            if (!roleResult.Succeeded)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            var claimsResult = await UpdateClaimsAsync(user, vm.UserRole == OakConstants.Roles.NotInRole ? Enumerable.Empty<string>() : vm.Boards.Where(b => b.IsChecked).Select(x => x.Item));
            if (!claimsResult.Succeeded)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return RedirectToAction(nameof(EditStaff), new { vm.UserId });
        }

        [HttpGet]
        public async Task<IActionResult> SelectUser(int page = 1)
        {
            const int usersPerPage = 50;
            var count = userManager.Users.Count();
            var pages = (int)Math.Ceiling((double)count / usersPerPage);
            var users = await userManager.Users
                .OrderBy(x => x.UserName)
                .Skip((page - 1) * usersPerPage)
                .Take(usersPerPage)
                .ToListAsync();
            return View(new SelectUserViewModel
            {
                Users = users,
                PagesInfo = new PaginatorViewModel
                {
                    TotalPages = pages,
                    PageNumber = page
                }
            });
        }

        [NonAction]
        private async Task<IdentityResult> UpdateUserRoleAsync(ApplicationUser user, string newRole)
        {
            IdentityResult result = IdentityResult.Success;
            var userRoles = await userManager.GetRolesAsync(user);
            var rolesToDelete = userRoles.Where(r => r != newRole);
            if (rolesToDelete.Any())
            {
                result = await userManager.RemoveFromRolesAsync(user, rolesToDelete);
            }

            if (result.Succeeded && !userRoles.Contains(newRole, StringComparer.InvariantCultureIgnoreCase) && newRole != OakConstants.Roles.NotInRole)
            {
                result = await userManager.AddToRoleAsync(user, newRole);
            }
            await modLogs.LogAsync(ApplicationEvent.AccountChangeRole, user.Id.ToString(), $"-- {string.Join(",", rolesToDelete)} ++ {newRole}");
            return result;
        }

        [NonAction]
        private async Task<IdentityResult> UpdateClaimsAsync(ApplicationUser user, IEnumerable<string> boards)
        {
            IdentityResult result = IdentityResult.Success;
            var userClaims = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == OakConstants.ClaimTypes.BoardPermission).ToList();

            var claimsToAdd = boards.Where(b => !userClaims.Any(uc => uc.Value == b)).Select(x => new Claim(OakConstants.ClaimTypes.BoardPermission, x));
            var claimsToRemove = userClaims.Where(c => !boards.Any(x => x == c.Value));
            if (claimsToRemove.Any())
            {
                result = await userManager.RemoveClaimsAsync(user, claimsToRemove);
            }
            if (result.Succeeded && claimsToAdd.Any())
            {
                result = await userManager.AddClaimsAsync(user, claimsToAdd);
            }
            await modLogs.LogAsync(ApplicationEvent.AccountChangeBoardsPermission, user.Id.ToString(), $"-- {string.Join(",", claimsToRemove)} ++ {string.Join(",", claimsToAdd)}");
            return result;
        }
    }

}
