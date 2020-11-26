using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public class DbUserService : IUserService
    {
        private readonly OakDbContext context;

        public DbUserService(OakDbContext context)
        {
            this.context = context;
        }

        public async Task<Anonymous> CreateAnonymousAsync(string ip)
        {
            var anon = new Anonymous
            {
                Created = DateTime.Now,
                Id = Guid.NewGuid(),
                IP = ip
            };

            context.Anonymous.Add(anon);
            await context.SaveChangesAsync();
            return anon;
        }
    }
}
