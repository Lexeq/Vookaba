#nullable enable
using System;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vookaba.Common;
using Vookaba.Common.Extensions;
using Vookaba.Services.Abstractions;
using Vookaba.Services.DTO;
using Vookaba.ViewModels.Ban;
using Vookaba.ViewModels.Post;

namespace Vookaba.Controllers.Api.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/{controller}")]
    public class PostsController : ControllerBase
    {
        private protected static int ChoosecSubnet(AddressFamily family)
        {
            return family == AddressFamily.InterNetwork ? 32 : 64;
        }

        private readonly IPostService posts;
        private readonly IBanService bans;
        private readonly IAuthorizationService authorization;
        private readonly IModLogService modLog;

        public PostsController(IPostService posts,
                               IBanService bans,
                               IAuthorizationService authorization,
                               IModLogService modLog)
        {
            this.posts = posts;
            this.bans = bans;
            this.authorization = authorization;
            this.modLog = modLog;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] PostsDeletionOptions vm)
        {
            var canDeleteResult = await authorization.AuthorizeAsync(User, vm, ApplicationConstants.Policies.CanDeletePosts);
            if (!canDeleteResult.Succeeded)
            {
                return Problem(statusCode: StatusCodes.Status403Forbidden,
                               title: "Permission denied.");
            }

            var post = await posts.GetByNumberAsync(vm.Board, vm.PostNumber);
            if (post is null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                               title: "Post not found.");
            }

            if (vm.Ban is not null)
            {
                var canBanResult = await authorization.AuthorizeAsync(
                    user: User,
                    resource: new BanParams
                    {
                        Board = vm.Ban.AllBoard == true ? ApplicationConstants.AllBoardsMark : vm.Board,
                        IsIPBan = vm.IPMode,
                        IsSubnetBan = false
                    },
                   policyName: ApplicationConstants.Policies.CanBanUsers);

                if (!canBanResult.Succeeded)
                {
                    return Problem(statusCode: StatusCodes.Status403Forbidden, title: "Permission denied");
                }

                var res = DateTime.UtcNow.TryAddFromString(vm.Ban.Duration, out var banExp);
                if (!res)
                {
                    return Problem(statusCode: StatusCodes.Status400BadRequest,
                                    title: "Bad ban duration");
                }

                BanCreationDto ban = new()
                {
                    Expired = banExp,
                    Board = vm.Area == PostsDeletionOptions.DeletingArea.All ? ApplicationConstants.AllBoardsMark : vm.Board,
                    BannedAothorToken = post.AuthorId,
                    BannedNetwork = vm.IPMode ? (post.AuthorIP, ChoosecSubnet(post.AuthorIP.AddressFamily)) : null,
                    PostId = post.PostId,
                    Reason = vm.Reason
                };
                await bans.CreateAsync(ban);
                await modLog.LogAsync(ApplicationEvent.BanCreated, "0");
            }


            if (vm.Area == PostsDeletionOptions.DeletingArea.Single)
            {
                await posts.DeleteByIdAsync(post.PostId);
                await modLog.LogAsync(
                   ApplicationEvent.PostDelete,
                   post.PostId.ToString(),
                   JsonSerializer.Serialize(new { vm.Reason }));
            }
            else
            {
                await posts.DeleteManyAsync(post.PostId, vm.IPMode ? Mode.Token | Mode.IP : Mode.Token, (SearchArea)vm.Area); ;
                await modLog.LogAsync(
                    ApplicationEvent.PostBulkDelete,
                    post.PostId.ToString(),
                    JsonSerializer.Serialize(new
                    {
                        vm.Reason,
                        vm.IPMode,
                        vm.Area,
                        AuthorIp = post.AuthorIP.ToString(),
                        post.AuthorId
                    }));
            }

            return Ok(new { Input = vm });
        }
    }
}
