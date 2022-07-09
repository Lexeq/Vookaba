using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vookaba.DAL.Database;
using Vookaba.Services.DTO;
using Vookaba.Services.Abstractions;

namespace Vookaba.Services.DbServices
{
    public class DbStaffAggregationService : IStaffAggregationService
    {
        private readonly VookabaDbContext context;

        public DbStaffAggregationService(VookabaDbContext context, IModLogService modLogs, ILogger<DbStaffAggregationService> logger)
        {
            this.context = context;
        }
        public async Task<ICollection<StaffDTO>> GetStaffAsync()
        {
            var staff = await (from ur in context.UserRoles
                               join usr in context.Users on ur.UserId equals usr.Id
                               join role in context.Roles on ur.RoleId equals role.Id
                               select new StaffDTO
                               {
                                   Id = usr.Id,
                                   Name = usr.UserName,
                                   Email = usr.Email,
                                   Role = role.Name,
                                   Boards = (from uc in context.UserClaims
                                             where uc.UserId == usr.Id && uc.ClaimType == Common.ApplicationConstants.ClaimTypes.BoardPermission
                                             select uc.ClaimValue).ToList()
                               }).ToListAsync();
            return staff;
        }
    }
}
