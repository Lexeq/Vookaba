using Microsoft.AspNetCore.Mvc;
using Vookaba.ViewModels;
using System.Net;
using System.Security.Claims;

namespace Vookaba.Controllers.Base
{
    public class AppMvcBaseController : Controller
    {
        public IPAddress IP => HttpContext.Connection.RemoteIpAddress;

        public string UserAgent => HttpContext.Request.Headers["User-Agent"];

        public string UserRole
        {
            get
            {
                return User.FindFirstValue(ClaimsIdentity.DefaultRoleClaimType) ?? Common.ApplicationConstants.Roles.NotInRole;
            }
        }

        protected virtual ViewResult Error(int code, string caption)
            => Error(code, caption, string.Empty);

        protected virtual ViewResult Error(int code, string caption, string message)
        {
            var vm = new ErrorViewModel(code, caption, message);
            HttpContext.Response.StatusCode = code;
            return View("Error", vm);
        }
    }
}
