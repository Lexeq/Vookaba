using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vookaba.Areas.Administration.ViewModels;
using Vookaba.Common;
using Vookaba.Controllers.Base;
using System.Linq;
using System.Threading.Tasks;
using Vookaba.Services.Abstractions;

namespace Vookaba.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = ApplicationConstants.Roles.Administrator)]
    [AutoValidateAntiforgeryToken]
    public class AdminController : AppMvcBaseController
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
                    Administrators = staff[ApplicationConstants.Roles.Administrator],
                    Moderators = staff[ApplicationConstants.Roles.Moderator],
                    Janitors = staff[ApplicationConstants.Roles.Janitor]
                }
            };

            return View(dash);
        }
    }

}