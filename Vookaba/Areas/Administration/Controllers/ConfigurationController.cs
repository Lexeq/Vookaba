using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vookaba.Areas.Administration.ViewModels;
using Vookaba.Common;
using Vookaba.Utils;

namespace Vookaba.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = ApplicationConstants.Roles.Administrator)]
    public class ConfigurationController : Controller
    {
        private readonly ApplicationOptions appOptions;
        private readonly OptionsRewriter rewriter;
        private readonly IMapper mapper;

        public ConfigurationController(IOptions<ApplicationOptions> appOptions,
                                 OptionsRewriter rewriter,
                                 IMapper mapper)
        {
            this.appOptions = appOptions.Value;
            this.rewriter = rewriter;
            this.mapper = mapper;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var vm = mapper.Map<AppConfiguratonViewModel>(appOptions);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Update(AppConfiguratonViewModel vm)
        {
            var opt = mapper.Map(vm, appOptions);
            rewriter.Write(opt);
            return RedirectToAction(nameof(AdminController.Dashboard), "Admin");
        }
    }
}
