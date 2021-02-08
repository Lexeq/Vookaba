using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OakChan.DAL.Database;
using OakChan.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OakChan.Utils
{
    public class DatabaseSeeder
    {
        private OakDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SeedData options;

        public DatabaseSeeder(OakDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<SeedData> options)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.options = options.Value;
        }

        public async Task SeedAsync()
        {
            var adminRole = await roleManager.FindByNameAsync(options.AdminRoleName);
            if (adminRole == null)
            {
                adminRole = new ApplicationRole(options.AdminRoleName);
                var result = await roleManager.CreateAsync(adminRole);
                if (!result.Succeeded)
                {
                    throw new Exception($"Can't create admin role: {string.Join(", ", result.Errors.Select(e => $"{e.Code}:{e.Description}"))}");
                }
            }

            var adminUserToRole = await context.UserRoles.FirstOrDefaultAsync(ur => ur.RoleId == adminRole.Id);

            if (adminUserToRole == null)
            {
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
    }
}
