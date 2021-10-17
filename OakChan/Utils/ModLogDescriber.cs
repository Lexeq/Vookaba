using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using OakChan.Areas.Administration.Controllers;
using OakChan.Common;
using OakChan.Services.DTO;

namespace OakChan.Utils
{
    public class ModLogDescriber
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly LinkGenerator linkGenerator;
        private readonly IStringLocalizer<ModLogDescriber> stringLocalizer;

        public ModLogDescriber(
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator,
            IStringLocalizer<ModLogDescriber> stringLocalizer)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.linkGenerator = linkGenerator;
            this.stringLocalizer = stringLocalizer;
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

                ApplicationEvent.InvitationCreate => stringLocalizer["Create invite {0}", log.EntityId],
                _ => stringLocalizer[$"{log.EventId} {log.EntityId} {log.Note}"]
            };
        }

        private string GetBoardEditionTag(string key)
            => $"<a href=\"{GetBoardEditionLink(key)}\">{key}</a>";

        private string GetBoardEditionLink(string key)
        {
            return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext,
                action: nameof(AdminController.EditBoard),
                controller: "Admin",
                new { Area = nameof(Areas.Administration), BoardKey = key });
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
    }
}
