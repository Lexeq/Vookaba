using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OakChan.DAL.Database;
using OakChan.Services.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public class DbStaffAggregationService : IStaffAggregationService
    {
        private readonly OakDbContext context;

        public DbStaffAggregationService(OakDbContext context, IModLogService modLogs, ILogger<DbStaffAggregationService> logger)
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
                                             where uc.UserId == usr.Id && uc.ClaimType == Common.OakConstants.ClaimTypes.BoardPermission
                                             select uc.ClaimValue).ToList()
                               }).ToListAsync();
            return staff;
        }
    }
}
