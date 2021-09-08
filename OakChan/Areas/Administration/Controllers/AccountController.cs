using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OakChan.Areas.Administration.ViewModels;
using OakChan.Common;
using OakChan.Identity;
using System.Threading.Tasks;

namespace OakChan.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class AccountController : Controller
    {
        private readonly ApplicationUserManager userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ChanOptions chanOptions;
        private readonly IStringLocalizer<AccountController> localizer;

        public AccountController(ApplicationUserManager userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<ChanOptions> chanOptionsAccessor,
            IStringLocalizer<AccountController> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.chanOptions = chanOptionsAccessor.Value;
            this.localizer = localizer;
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

        private IActionResult RedirectToReturnUrlOrDefault(string returnUrl) =>
            string.IsNullOrEmpty(returnUrl) ?
                RedirectToRoute("default") :
                (IActionResult)LocalRedirect(returnUrl);
    }
}
