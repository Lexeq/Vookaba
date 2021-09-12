using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OakChan.Areas.Administration.ViewModels;
using OakChan.Common;
using OakChan.Identity;
using OakChan.Utils;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OakChan.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class AccountController : Controller
    {
        private readonly ApplicationUserManager userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ChanOptions chanOptions;
        private readonly ClaimsIdentityOptions claimOptions;
        private readonly InvitationManager<ApplicationInvitation> invitations;
        private readonly IStringLocalizer<AccountController> localizer;
        private readonly ILogger<AccountController> logger;

        public AccountController(ApplicationUserManager userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<ChanOptions> chanOptionsAccessor,
            IOptions<ClaimsIdentityOptions> claimOptionsAccessor,
            InvitationManager<ApplicationInvitation> invitations,
            IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.invitations = invitations;
            this.chanOptions = chanOptionsAccessor.Value;
            this.claimOptions = claimOptionsAccessor.Value;
            this.localizer = localizer;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Register(string invitation)
        {
            return View(chanOptions.PublicRegistrationEnabled ?
                new RegisterViewModel() :
                new RegisterWithInvitationViewModel { Invitaion = invitation });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm, string returnUrl = null)
        {
            if (!chanOptions.PublicRegistrationEnabled)
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

                IdentityResult result = chanOptions.PublicRegistrationEnabled ?
                    await userManager.CreateAsync(user, vm.Password) :
                    await userManager.CreateAsync(user, vm.Password, (vm as RegisterWithInvitationViewModel).Invitaion);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToReturnUrlOrDefault(returnUrl);
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await signInManager.PasswordSignInAsync(vm.Login, vm.Password, vm.Remember, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToReturnUrlOrDefault(returnUrl);
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
        [Authorize(Policy = "CanInvite")]
        [ValidateAntiForgeryToken]
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

        private IActionResult RedirectToReturnUrlOrDefault(string returnUrl) =>
            string.IsNullOrEmpty(returnUrl) ?
                RedirectToRoute("default") :
                (IActionResult)LocalRedirect(returnUrl);
    }
}
