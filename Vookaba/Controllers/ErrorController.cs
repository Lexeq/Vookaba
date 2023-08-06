using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vookaba.Controllers.Base;
using Vookaba.Utils;

namespace Vookaba.Controllers
{
    [AllowAnonymous]
    public class ErrorController : AppMvcBaseController
    {
        private readonly ILogger<ErrorController> logger;
        private readonly HttpStatusCodeDescriber describer;
        public ErrorController(ILogger<ErrorController> logger, HttpStatusCodeDescriber describer)
        {
            this.logger = logger;
            this.describer = describer;
        }

        public IActionResult HandleHttpStatusCode(int statusCode)
        {
            if (!IsRequestFromReExecuteMiddleware)
            {
                return Error(404);
            }

            var original = HttpContext.Features.Get<IStatusCodeReExecuteFeature>().OriginalPath;
            var isFromApi = original.StartsWith("/api/v", System.StringComparison.OrdinalIgnoreCase);

            return isFromApi ? Problem(statusCode: statusCode) : Error(statusCode);
        }

        public IActionResult HandleException()
        {
            if (!IsRequestFromExceptionMiddleware)
            {
                return Error(404);
            }
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError(exceptionFeature.Error, $"An error has occured. Path: {exceptionFeature.Path}. Query: {HttpContext.Request.QueryString}.");
            var fromApi = exceptionFeature.Path.StartsWith("/api/v", System.StringComparison.OrdinalIgnoreCase);
            return fromApi ? Problem(statusCode: StatusCodes.Status500InternalServerError) : Error(HttpContext.Response.StatusCode);
        }

        private IActionResult Error(int statusCode) =>
            base.Error(statusCode, describer.GetStatusCodeDescription(statusCode));

        private bool IsRequestFromExceptionMiddleware => HttpContext.Features.Get<IExceptionHandlerPathFeature>() != null;

        private bool IsRequestFromReExecuteMiddleware => HttpContext.Features.Get<IStatusCodeReExecuteFeature>() != null;
    }
}
