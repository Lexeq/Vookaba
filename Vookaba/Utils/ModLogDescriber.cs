using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Vookaba.Areas.Administration.Controllers;
using Vookaba.Common;
using Vookaba.Controllers;
using Vookaba.Services.DTO;
using System.Text.Json;

namespace Vookaba.Utils
{
    public class ModLogDescriber
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly LinkGenerator linkGenerator;
        private readonly IStringLocalizer<ModLogDescriber> stringLocalizer;
        private readonly ILogger<ModLogDescriber> logger;

        public ModLogDescriber(
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator,
            IStringLocalizer<ModLogDescriber> stringLocalizer,
            ILogger<ModLogDescriber> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.linkGenerator = linkGenerator;
            this.stringLocalizer = stringLocalizer;
            this.logger = logger;
        }

        public string ToHtmlString(ModLogDto log)
        {
            return log.EventId switch
            {
                ApplicationEvent.BoardCreate => stringLocalizer["Create board /{0}/.", GetBoardEditionTag(log.EntityId)],
                ApplicationEvent.BoardDelete => stringLocalizer["Delete board /{0}/.", log.EntityId],
                ApplicationEvent.BoardEdit => stringLocalizer["Edit board /{0}/.", GetBoardEditionTag(log.EntityId)],

                ApplicationEvent.AccountBoardsPermissionAdd => stringLocalizer["Add permission to boards /{0}/ to user {1}", log.Note, GetUsetEditionTag(log.EntityId)],
                ApplicationEvent.AccountBoardsPermissionRemove => stringLocalizer["Remove permission to boards /{0}/ from user {1}", log.Note, GetUsetEditionTag(log.EntityId)],
                ApplicationEvent.AccountCreate => stringLocalizer["User registred."],
                ApplicationEvent.AccountLogin => stringLocalizer["User logged in."],
                ApplicationEvent.AccountRoleAdd => stringLocalizer["Add role `{0}` to user {1}", log.Note, GetUsetEditionTag(log.EntityId)],
                ApplicationEvent.AccountRoleRemove => stringLocalizer["Remove role `{0}` from user {1}", log.Note, GetUsetEditionTag(log.EntityId)],

                ApplicationEvent.ThreadIsPinnedChanged => stringLocalizer[$"{(log.Note == "true" ? "Pin": "Unpin")} thread #{{0}}", log.EntityId],
                ApplicationEvent.ThreadIsLockedChanged => stringLocalizer[$"{(log.Note == "true" ? "Lock": "Unlock")} thread #{{0}}", log.EntityId],

                ApplicationEvent.PostDelete => stringLocalizer["Delete post #{0} ({1})", log.EntityId, GetDeletionReason(log.Note)],
                ApplicationEvent.PostBulkDelete => stringLocalizer["Delete many posts ({0})", GetDeletionReason(log.Note)],

                ApplicationEvent.InvitationCreate => stringLocalizer["Create invite {0}", log.EntityId],
                _ => stringLocalizer[$"{log.EventId} {log.EntityId} {log.Note}"]
            };
        }

        private string GetBoardEditionTag(string key)
            => $"<a href=\"{GetBoardEditionLink(key)}\">{key}</a>";

        private string GetBoardEditionLink(string key)
        {
            return linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext,
                routeName: "boardAction",
                new { board = key, action = nameof(BoardController.Edit) });
        }

        private string GetUsetEditionTag(string id)
            => $"<a href=\"{GetUserEditionLink(id)}\">#{id}</a>";

        private string GetUserEditionLink(string id)
        {
            return linkGenerator.GetPathByAction(
                httpContextAccessor.HttpContext,
                action: nameof(AccountController.UserDetails),
                controller: "Account",
                values: new { UserId = id });
        }

        private string GetDeletionReason(string json)
        {
            try
            {
                return JsonDocument.Parse(json)
                    .RootElement
                    .GetProperty("Reason")
                    .GetString();
            }
            catch (JsonException e)
            {
                logger.LogWarning(e, "Can't parse log note.");
                return "N/A";
            }
        }
    }
}
