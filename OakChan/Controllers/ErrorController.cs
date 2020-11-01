using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OakChan.ViewModels;

namespace OakChan.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;
        private readonly IStringLocalizer<ErrorController> localizer;
        public ErrorController(ILogger<ErrorController> logger, IStringLocalizer<ErrorController> localizer)
        {
            this.logger = logger;
            this.localizer = localizer;
        }

        public IActionResult HandleHttpStatusCode(int statusCode) =>
            IsRequestFromReExecuteMiddleware ? ErrorView(statusCode) : ErrorView(404);

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
            this.ErrorView(new ErrorViewModel
            {
                Code = statusCode,
                Title = localizer[GetStatusCodeDescription(statusCode)]
            });

        private string GetStatusCodeDescription(int code) =>
           code switch
           {
               100 => "Continue",
               101 => "Switching Protocols",
               102 => "Processing",
               200 => "OK",
               201 => "Created",
               202 => "Accepted",
               203 => "Non-Authoritative Information",
               204 => "No Content",
               205 => "Reset Content",
               206 => "Partial Content",
               207 => "Multi-Status",
               300 => "Multiple Choices",
               301 => "Moved Permanently",
               302 => "Found",
               303 => "See Other",
               304 => "Not Modified",
               305 => "Use Proxy",
               307 => "Temporary Redirect",
               400 => "Bad Request",
               401 => "Unauthorized",
               402 => "Payment Required",
               403 => "Forbidden",
               404 => "Not Found",
               405 => "Method Not Allowed",
               406 => "Not Acceptable",
               407 => "Proxy Authentication Required",
               408 => "Request Timeout",
               409 => "Conflict",
               410 => "Gone",
               411 => "Length Required",
               412 => "Precondition Failed",
               413 => "Request Entity Too Large",
               414 => "Request-Uri Too Long",
               415 => "Unsupported Media Type",
               416 => "Requested Range Not Satisfiable",
               417 => "Expectation Failed",
               422 => "Unprocessable Entity",
               423 => "Locked",
               424 => "Failed Dependency",
               500 => "Internal Server Error",
               501 => "Not Implemented",
               502 => "Bad Gateway",
               503 => "Service Unavailable",
               504 => "Gateway Timeout",
               505 => "Http Version Not Supported",
               507 => "Insufficient Storage",
               _ => "Unknown Error"
           };

        private bool IsRequestFromExceptionMiddleware => HttpContext.Features.Get<IExceptionHandlerPathFeature>() != null;

        private bool IsRequestFromReExecuteMiddleware => HttpContext.Features.Get<IStatusCodeReExecuteFeature>() != null;
    }
}
