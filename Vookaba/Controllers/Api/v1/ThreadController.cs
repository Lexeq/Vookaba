using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vookaba.Common;
using Vookaba.Services;
using System.Threading.Tasks;

namespace Vookaba.Controllers.Api.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/{controller}")]
    public class ThreadsController : ControllerBase
    {
        private readonly IThreadService threads;
        private readonly IAuthorizationService authorization;
        private readonly IModLogService modLogs;

        public ThreadsController(IThreadService threads,
                                 IAuthorizationService authorization,
                                 IModLogService modLogs)
        {
            this.threads = threads;
            this.authorization = authorization;
            this.modLogs = modLogs;
        }

        [HttpPost]
        [Route("pin")]
        public Task<IActionResult> Pin([FromQuery] string board, [FromQuery] int thread)
            => SetIsPinned(board, thread, true);

        [HttpPost]
        [Route("unpin")]
        public Task<IActionResult> Unpin([FromQuery] string board, [FromQuery] int thread)
            => SetIsPinned(board, thread, false);

        [HttpPost]
        [Route("lock")]
        public Task<IActionResult> Lock([FromQuery] string board, [FromQuery] int thread)
            => SetIsLocked(board, thread, true);

        [HttpPost]
        [Route("unlock")]
        public Task<IActionResult> Unlock([FromQuery] string board, [FromQuery] int thread)
            => SetIsLocked(board, thread, false);

        private async Task<IActionResult> SetIsPinned(string board, int threadId, bool isPined)
        {
            var authResult = await authorization.AuthorizeAsync(User, null, ApplicationConstants.Policies.CanEditThreads);
            if (!authResult.Succeeded)
            {
                return Problem(statusCode: StatusCodes.Status403Forbidden,
                               title: "Permission denied.");
            }
            var t = await threads.GetThreadInfoAsync(board, threadId);
            if (t == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound);
            }

            await threads.SetIsPinned(t.ThreadId, isPined);
            await modLogs.LogAsync(ApplicationEvent.ThreadIsPinnedChanged, threadId.ToString(), isPined.ToString());
            return Ok();
        }

        private async Task<IActionResult> SetIsLocked(string board, int threadId, bool isLocked)
        {
            var authResult = await authorization.AuthorizeAsync(User, null, ApplicationConstants.Policies.CanEditThreads);
            if (!authResult.Succeeded)
            {
                return Problem(statusCode: StatusCodes.Status403Forbidden,
                               title: "Permission denied.");
            }
            var t = await threads.GetThreadInfoAsync(board, threadId);
            if (t == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound);
            }

            await threads.SetIsReadOnly(t.ThreadId, isLocked);
            await modLogs.LogAsync(ApplicationEvent.ThreadIsLockedChanged, threadId.ToString(), isLocked.ToString());
            return Ok();
        }
    }
}
