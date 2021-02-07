using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OakChan.Areas.Administration.ViewModels;
using OakChan.Identity;
using System.Threading.Tasks;

namespace OakChan.Areas.Administration.Controllers
{
    [Area("Administration")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IStringLocalizer<AccountController> localizer;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<AccountController> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.localizer = localizer;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = vm.Email,
                    UserName = vm.Login
                };

                var result = await userManager.CreateAsync(user, vm.Password);
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
