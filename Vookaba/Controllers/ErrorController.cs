using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Vookaba.Controllers.Base;
using Vookaba.Utils;
using Vookaba.ViewModels;

namespace Vookaba.Controllers
{
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

            if (IsRequestFromReExecuteMiddleware)
            {
                var original = HttpContext.Features.Get<IStatusCodeReExecuteFeature>().OriginalPath;
                return original.StartsWith("/api/v", System.StringComparison.OrdinalIgnoreCase) ? StatusCode(statusCode) : ErrorView(statusCode);
            }
            else
            {
                return ErrorView(404);
            }
        }

        public IActionResult HandleException()
        {
            if (!IsRequestFromExceptionMiddleware)
            {
                return ErrorView(404);
            }
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.LogError(exceptionFeature.Error, $"An error has occured. Path: {exceptionFeature.Path}. Query: {HttpContext.Request.QueryString}.");
            return ErrorView(HttpContext.Response.StatusCode);
        }

        private IActionResult ErrorView(int statusCode) =>
            base.Error(statusCode, describer.GetStatusCodeDescription(statusCode));

        private bool IsRequestFromExceptionMiddleware => HttpContext.Features.Get<IExceptionHandlerPathFeature>() != null;

        private bool IsRequestFromReExecuteMiddleware => HttpContext.Features.Get<IStatusCodeReExecuteFeature>() != null;
    }
}
