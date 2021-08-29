using Microsoft.AspNetCore.Mvc;
using OakChan.ViewModels;
using System.Net;

namespace OakChan.Controllers.Base
{
    public class OakController : Controller
    {
        public IPAddress IP => HttpContext.Connection.RemoteIpAddress;

        public string UserAgent => HttpContext.Request.Headers["User-Agent"];

        protected virtual ViewResult Error(int code, string caption)
            => Error(code, caption, string.Empty);

        protected virtual ViewResult Error(int code, string caption, string message)
        {
            var vm = new ErrorViewModel(code, caption, message);

            return View("Error", vm);
        }
    }
}
