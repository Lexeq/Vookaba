using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OakChan.Tests.Base
{
    public class Seeder
    {
        private readonly DbContextOptions<OakDbContext> dbContextOptions;

        public DbSeedData Data { get; } = new DbSeedData();

        public bool Seeded { get; private set; }

        public Seeder(DbContextOptions<OakDbContext> dbContextOptions)
        {
            this.dbContextOptions = dbContextOptions;
        }
        public void Seed()
        {
            if(Seeded)
            {
                throw new InvalidOperationException("The database is already seeded.");
            }

            using var db = new OakDbContext(dbContextOptions);

            if (HasEnteties(Data.Tokens)) db.AnonymousTokens.AddRange(Data.Tokens);
            if (HasEnteties(Data.Boards)) db.Boards.AddRange(Data.Boards);
            if (HasEnteties(Data.Threads)) db.Threads.AddRange(Data.Threads);
            if (HasEnteties(Data.Posts)) db.Posts.AddRange(Data.Posts);
            if (HasEnteties(Data.Images)) db.Images.AddRange(Data.Images);

            db.SaveChanges();
            Seeded = true;
        }

        private bool HasEnteties<T>(IEnumerable<T> entities)
            => entities != null && entities.Any();
    }
}
