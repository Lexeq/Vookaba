using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vookaba.Common;
using Vookaba.Services;
using Vookaba.ViewModels;
using System.Text.Json;
using System.Threading.Tasks;

namespace Vookaba.Controllers.Api.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/{controller}")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService posts;
        private readonly IAuthorizationService authorization;
        private readonly IModLogService modLog;

        public PostsController(IPostService posts,
                               IAuthorizationService authorization,
                               IModLogService modLog)
        {
            this.posts = posts;
            this.authorization = authorization;
            this.modLog = modLog;
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] PostsDeletionOptions vm)
        {
            var authResult = await authorization.AuthorizeAsync(User, vm, ApplicationConstants.Policies.CanDeletePosts);
            if (!authResult.Succeeded)
            {
                return Problem(statusCode: StatusCodes.Status403Forbidden,
                               title: "Permission denied.");
            }

            var post = await posts.GetByNumberAsync(vm.Board, vm.PostNumber);
            if (post == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                               title: "Post not found.");
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
                await posts.DeleteManyAsync(post.PostId, vm.Mode.Value, (SearchArea)vm.Area);

                await modLog.LogAsync(
                    ApplicationEvent.PostBulkDelete,
                    post.PostId.ToString(),
                    JsonSerializer.Serialize(new
                    {
                        vm.Reason,
                        vm.Mode,
                        vm.Area,
                        AuthorIp = post.AuthorIP.ToString(),
                        post.AuthorId
                    }));
            }
            return Ok();
        }
    }
}
