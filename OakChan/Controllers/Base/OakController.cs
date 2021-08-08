using Microsoft.AspNetCore.Mvc;
using OakChan.ViewModels;

namespace OakChan.Controllers.Base
{
    public class OakController : Controller
    {
        protected virtual ViewResult Error(int code, string caption, string message)
        {
            var vm = new ErrorViewModel(code, caption, message);

            return View("Error", vm);
        }
    }
}
