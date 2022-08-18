using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Vookaba.Common;
using Vookaba.Services.Abstractions;
using Vookaba.ViewModels.Error;

namespace Vookaba.Security
{
    public class BannedFilter : IAsyncAuthorizationFilter
    {
        private readonly IBanService bans;
        private readonly ILogger logger;

        public BannedFilter(IBanService bans, ILogger<BannedFilter> logger)
        {
            this.bans = bans;
            this.logger = logger;
        }


        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var board = context.RouteData.Values["board"]?.ToString();
            if (board == null)
            {
                logger.NoBoardValue();
                context.Result = new BadRequestResult();
                return;
            }
            var ip = context.HttpContext.Connection.RemoteIpAddress;
            var token = context.HttpContext.User.FindFirstValue(ApplicationConstants.ClaimTypes.AuthorToken);
            var ban = await bans.FindActiveBan(ip, Guid.Parse(token), board);
            if (ban is null)
            {
                return;
            }

            var model = new ErrorViewModel
            {
                Code = StatusCodes.Status403Forbidden,
                Title = "Access Denied",
                Description = $"You are banned until {ban.Expired} UTC. (ID: {ban.Id})"
            };

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };
            var result = new ViewResult
            {
                ViewData = viewData,
                ViewName = "Error"
            };

            context.Result = result;
        }
    }
}
