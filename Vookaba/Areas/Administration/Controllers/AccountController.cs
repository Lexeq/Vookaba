using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vookaba.Areas.Administration.ViewModels;
using Vookaba.Common;
using Vookaba.Controllers.Base;
using Vookaba.Identity;
using Vookaba.Services;
using Vookaba.Utils;
using Vookaba.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;

namespace Vookaba.Areas.Administration.Controllers
{
    [Area("Administration")]
    [AutoValidateAntiforgeryToken]
    public class AccountController : AppMvcBaseController
    {
        private readonly ApplicationUserManager userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationOptions appOptions;
        private readonly ClaimsIdentityOptions claimOptions;
        private readonly InvitationManager<ApplicationInvitation> invitations;
        private readonly IBoardService boardService;
        private readonly IModLogService modLogs;
        private readonly IStringLocalizer<AccountController> localizer;
        private readonly ILogger<AccountController> logger;
        private readonly HttpStatusCodeDescriber statusCodeDescriber;

        public AccountController(ApplicationUserManager userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<ApplicationOptions> chanOptionsAccessor,
            IOptions<ClaimsIdentityOptions> claimOptionsAccessor,
            InvitationManager<ApplicationInvitation> invitations,
            IModLogService modLogs,
            IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger, IBoardService boardService,
            HttpStatusCodeDescriber statusCodeDescriber)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.invitations = invitations;
            this.modLogs = modLogs;
            this.appOptions = chanOptionsAccessor.Value;
            this.claimOptions = claimOptionsAccessor.Value;
            this.localizer = localizer;
            this.logger = logger;
            this.boardService = boardService;
            this.statusCodeDescriber = statusCodeDescriber;
        }

        [HttpGet]
        public IActionResult Register(string invitation)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToRoute("home");
            }
            return View(appOptions.RegistrationByInvitation ?
                new RegisterWithInvitationViewModel { Invitaion = invitation } :
                new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (appOptions.RegistrationByInvitation)
            {
                vm = new RegisterWithInvitationViewModel();
                await TryUpdateModelAsync(vm as RegisterWithInvitationViewModel);
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = vm.Email,
                    UserName = vm.Login
                };

                IdentityResult result = appOptions.RegistrationByInvitation ?
                    await userManager.CreateAsync(user, vm.Password, ((RegisterWithInvitationViewModel)vm).Invitaion) :
                    await userManager.CreateAsync(user, vm.Password);
                if (result.Succeeded)
                {
                    User.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }));
                    await modLogs.LogAsync(ApplicationEvent.AccountCreate, user.Id.ToString());
                    return View(nameof(Login), new LoginViewModel { Login = vm.Login });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(vm);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToRoute("home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(vm.Login);
                if (user != null)
                {
                    var signInResult = await signInManager.PasswordSignInAsync(user, vm.Password, vm.Remember, false);
                    if (signInResult.Succeeded)
                    {
                        User.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }));
                        await modLogs.LogAsync(ApplicationEvent.AccountLogin, user.Id.ToString());
                        return RedirectToReturnUrlOrDefault(returnUrl);
                    }
                }
                ModelState.AddModelError(string.Empty, localizer["Unsuccessful login attempt. Please check login and password."]);
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> LogOut(string returnUrl = null)
        {
            await signInManager.SignOutAsync();
            return RedirectToReturnUrlOrDefault(returnUrl);
        }

        [HttpPost]
        [Authorize(Policy = ApplicationConstants.Policies.CanInviteUsers)]
        public async Task<IActionResult> CreateInvitation([FromBody] int days)
        {
            var expirationDate = days < 1 ? DateTime.MaxValue : DateTime.UtcNow.AddDays(days);
            var invitation = new ApplicationInvitation
            {
                Expire = expirationDate,
                PublisherId = int.Parse(User.FindFirst(claimOptions.UserIdClaimType).Value)
            };

            try
            {
                await invitations.CreateAsync(invitation);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Can't create invitation.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            await modLogs.LogAsync(ApplicationEvent.InvitationCreate, invitation.Id.ToString());
            return Ok(new
            {
                Token = new
                {
                    Value = invitation.Token,
                    LocalizedParamName = localizer["Invitation token"].ToString()
                },
                Url = new
                {
                    Value = Url.ActionLink(action: nameof(Register), values: new { Invitation = invitation.Token }),
                    LocalizedParamName = localizer["Invitation URL"].ToString()
                },
                Expire = new
                {
                    Value = invitation.Expire.ToUniversalTime().GetUnixEpochOffset(),
                    LocalizedParamName = localizer["Invitation expire"].ToString()
                }
            });
        }

        [HttpGet]
        [Authorize(Policy = ApplicationConstants.Policies.CanEditUsers)]
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

        [HttpGet]
        [Authorize(Policy = ApplicationConstants.Policies.CanEditUsers)]
        public async Task<IActionResult> UserDetails(string userId, [FromQuery(Name = "page")] int logsPage = 1)
        {
            const int logsOnPage = 10;
            logsPage = logsPage < 1 ? 1 : logsPage;

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
                .Where(c => c.Type == ApplicationConstants.ClaimTypes.BoardPermission)
                .Select(x => x.Value)
                .ToHashSet();

            var logs = await modLogs.GetLogsForUserAsync(int.Parse(userId), (logsPage - 1) * logsOnPage, logsOnPage + 1);

            var isAdmin = await userManager.IsInRoleAsync(user, ApplicationConstants.Roles.Administrator);

            var vm = new UserDetailsViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Boards = isAdmin ? Array.Empty<CheckableItem<string>>() : (await boardService.GetBoardsAsync(true))
                      .Select(x => new CheckableItem<string>(x.Key, isAdmin || boards.Contains(x.Key)))
                      .ToList(),
                UserRole = isAdmin ? ApplicationConstants.Roles.Administrator : (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? ApplicationConstants.Roles.NotInRole,
                Roles = isAdmin ? new[] { ApplicationConstants.Roles.Administrator } : new[] { ApplicationConstants.Roles.NotInRole, ApplicationConstants.Roles.Janitor, ApplicationConstants.Roles.Moderator },
                Logs = logs.Take(logsOnPage),
                IsEditable = !isAdmin,
                PageInfo = new PaginatorViewModel
                {
                    PageNumber = logsPage,
                    TotalPages = logs.Count() > logsOnPage ? logsPage + 1 : logsPage
                }
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Policy = ApplicationConstants.Policies.CanEditUsers)]
        public async Task<IActionResult> UpdateUserPermissions(AssignRoleViewModel vm)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            ApplicationUser user = null;
            if (ModelState.IsValid && (user = await userManager.FindByIdAsync(vm.UserId)) != null)
            {
                var roleResult = await UpdateUserRoleAsync(user, vm.Role);
                if (roleResult.Succeeded)
                {
                    var claimsResult = await UpdateClaimsAsync(
                        user: user,
                        boards: vm.Role == ApplicationConstants.Roles.NotInRole ?
                            Enumerable.Empty<string>() :
                            vm.Boards.Where(b => b.IsChecked).Select(x => x.Item));
                    if (claimsResult.Succeeded)
                    {
                        await userManager.UpdateSecurityStampAsync(user);
                        scope.Complete();
                        return RedirectToAction(nameof(UserDetails), new { vm.UserId });
                    }
                    else
                    {
                        logger.LogError("Can't update claims: {errors}", claimsResult.Errors);
                    }
                }
                else
                {
                    logger.LogError("Can't update role: {errors}", roleResult.Errors);
                }
            }
            return BadRequest();
        }

        private async Task<IdentityResult> UpdateUserRoleAsync(ApplicationUser user, string newRole)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            if (userRoles.Contains(ApplicationConstants.Roles.Administrator))
            {
                return IdentityResult.Failed(new IdentityError() { Description = localizer["Can't edit admin user"] });
            }

            IdentityResult result = IdentityResult.Success;

            var rolesToDelete = userRoles.Where(r => r != newRole);
            if (rolesToDelete.Any())
            {
                result = await userManager.RemoveFromRolesAsync(user, rolesToDelete);
                await modLogs.LogAsync(ApplicationEvent.AccountRoleRemove, user.Id.ToString(), string.Join(", ", rolesToDelete));
            }

            if (result.Succeeded && !userRoles.Contains(newRole, StringComparer.InvariantCultureIgnoreCase) && newRole != ApplicationConstants.Roles.NotInRole)
            {
                result = await userManager.AddToRoleAsync(user, newRole);
                await modLogs.LogAsync(ApplicationEvent.AccountRoleAdd, user.Id.ToString(), newRole);

            }
            return result;
        }

        private async Task<IdentityResult> UpdateClaimsAsync(ApplicationUser user, IEnumerable<string> boards)
        {
            IdentityResult result = IdentityResult.Success;
            var userClaims = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == ApplicationConstants.ClaimTypes.BoardPermission).ToList();

            var claimsToRemove = userClaims.Where(c => !boards.Any(x => x == c.Value));
            if (claimsToRemove.Any())
            {
                result = await userManager.RemoveClaimsAsync(user, claimsToRemove);
                await modLogs.LogAsync(ApplicationEvent.AccountBoardsPermissionRemove, user.Id.ToString(), string.Join(", ", claimsToRemove.Select(c => c.Value)));
            }
            var claimsToAdd = boards
                .Where(b => !userClaims.Any(uc => uc.Value == b))
                .Select(x => new Claim(ApplicationConstants.ClaimTypes.BoardPermission, x));
            if (result.Succeeded && claimsToAdd.Any())
            {
                result = await userManager.AddClaimsAsync(user, claimsToAdd);
                await modLogs.LogAsync(ApplicationEvent.AccountBoardsPermissionAdd, user.Id.ToString(), string.Join(", ", claimsToAdd.Select(c => c.Value)));
            }

            return result;
        }

        private IActionResult RedirectToReturnUrlOrDefault(string returnUrl) =>
            string.IsNullOrEmpty(returnUrl) ?
                RedirectToRoute("home") :
                LocalRedirect(returnUrl);
    }
}
