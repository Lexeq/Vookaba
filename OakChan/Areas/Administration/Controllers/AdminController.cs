using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OakChan.Areas.Administration.ViewModels;
using OakChan.Common;
using OakChan.Controllers.Base;
using OakChan.Services;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = OakConstants.Roles.Administrator)]
    [AutoValidateAntiforgeryToken]
    public class AdminController : OakController
    {
        private readonly IBoardService boardService;
        private readonly IStaffAggregationService staff;

        public AdminController(IBoardService boardService,
            IStaffAggregationService staff)
        {
            this.boardService = boardService;
            this.staff = staff;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var staff = (await this.staff.GetStaffAsync()).ToLookup(s => s.Role);
            var dash = new AdminDashboardViemModel()
            {
                Boards = await boardService.GetBoardsAsync(true),
                Staff = new StuffViewModel
                {
                    Administrators = staff[OakConstants.Roles.Administrator],
                    Moderators = staff[OakConstants.Roles.Moderator],
                    Janitors = staff[OakConstants.Roles.Janitor]
                }
            };

            return View(dash);
        }
    }

}