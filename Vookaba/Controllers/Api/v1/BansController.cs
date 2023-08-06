#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vookaba.Common;
using Vookaba.Common.Extensions;
using Vookaba.Services.Abstractions;
using Vookaba.Services.DTO;
using Vookaba.ViewModels.Ban;

namespace Vookaba.Controllers.Api.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/{controller}")]
    public class BansController : ControllerBase
    {
        private readonly IBanService bans;
        private readonly IAuthorizationService authorization;

        public BansController(IBanService bans, IAuthorizationService authorization)
        {
            this.bans = bans;
            this.authorization = authorization;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(BanCreationViewModel banVm)
        {
            var auth = await authorization.AuthorizeAsync(
                User,
                new BanParams
                {
                    Board = banVm.Board,
                    IsIPBan = banVm.IP != null, 
                    IsSubnetBan = banVm.Subnet.HasValue
                },
                ApplicationConstants.Policies.CanBanUsers);
            if (!auth.Succeeded)
            {
                return Forbid();
            }
            var successed = DateTime.UtcNow.TryAddFromString(banVm.Duration, out var banExp);
            if (!successed)
            {
                return BadRequest();
            }

            BanCreationDto ban = new()
            {
                Expired = banExp,
                Board = banVm.Board,
                Reason = banVm.Reason
            };
            await bans.CreateAsync(ban);
            return Ok();
        }

        [HttpDelete("cancel")]
        public async Task<IActionResult> Cancel(int banId)
        {
            await bans.RemoveAsync(banId);
            return Ok();
        }
    }
}
