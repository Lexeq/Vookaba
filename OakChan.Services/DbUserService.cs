using AutoMapper;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public class DbUserService : IUserService
    {
        private readonly OakDbContext context;
        private readonly IMapper mapper;

        public DbUserService(OakDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UserDto> CreateAnonymousAsync(string ip)
        {
            var anon = new IdToken
            {
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                IP = ip
            };

            context.IdTokens.Add(anon);
            await context.SaveChangesAsync();
            var user = mapper.Map<UserDto>(anon);
            return user;
        }
    }
}
