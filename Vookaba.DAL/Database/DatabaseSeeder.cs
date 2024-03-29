﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vookaba.Common;
using Vookaba.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vookaba.DAL.Database
{
    public class DatabaseSeeder
    {
        private readonly VookabaDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ILogger<DatabaseSeeder> logger;
        private readonly SeedData options;

        public DatabaseSeeder(VookabaDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<SeedData> options,
            ILogger<DatabaseSeeder> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
            this.options = options.Value;
        }

        public async Task SeedAsync()
        {
            await SeedRoles();
            await SeedAdminUser();
            await SeedBoards();
        }

        private async Task SeedRoles()
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                var roles = new[]
                {
                    new ApplicationRole(ApplicationConstants.Roles.Administrator),
                    new ApplicationRole(ApplicationConstants.Roles.Moderator),
                    new ApplicationRole(ApplicationConstants.Roles.Janitor)
                };

                foreach (var role in roles)
                {
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Can't create role '{role.Name}': {string.Join(", ", result.Errors.Select(e => $"{e.Code} : {e.Description}"))}");
                    }
                }
            }
        }


        private async Task SeedAdminUser()
        {
            var adminRole = await roleManager.FindByNameAsync(ApplicationConstants.Roles.Administrator);
            var adminUserToRole = await context.UserRoles.FirstOrDefaultAsync(ur => ur.RoleId == adminRole.Id);

            if (adminUserToRole == null)
            {
                if (string.IsNullOrEmpty(options.AdministratorPassword) ||
                    string.IsNullOrEmpty(options.AdministratorUserName) ||
                    string.IsNullOrEmpty(options.AdministratorEmail))
                {
                    logger.LogWarning("No admin. Check configuration.");
                    return;
                }
                var adminUser = new ApplicationUser
                {
                    UserName = options.AdministratorUserName,
                    Email = options.AdministratorEmail,
                    EmailConfirmed = true,
                };
                var userCreateResult = await userManager.CreateAsync(adminUser, options.AdministratorPassword);
                if (!userCreateResult.Succeeded)
                {
                    throw new Exception($"Can't create admin user: {string.Join(", ", userCreateResult.Errors.Select(e => $"{e.Code}:{e.Description}"))}");
                }
                await userManager.AddToRoleAsync(adminUser, adminRole.Name);
            }
        }

        private async Task SeedBoards()
        {
            if (await context.Boards.AnyAsync())
            {
                return;
            }
            if (!options.Boards.Any())
            {
                logger.LogWarning("No default boards. Check configuration.");
                return;
            }

            context.Boards.AddRange(options.Boards);
            await context.SaveChangesAsync();
        }

    }
}
