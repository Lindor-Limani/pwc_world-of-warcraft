using Microsoft.EntityFrameworkCore;
using System;
using pwc.Infrastructure;

namespace pwc.Infrastructure.Test
{
    public class DatabaseTest
    {
        /*private const string TestConnectionString =
            "Host=localhost;Port=5432;Database=pwcdb;Username=pwc;Password=pwc;Include Error Detail=true";

        [Fact]
        public void CodeFirst_CreatesAndSeedsDatabase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(TestConnectionString)
                .Options;

            // Ensure database is clean and apply migrations
            using (var context = new AppDbContext(options))
            {
               //s context.Database.EnsureDeleted();
                context.Database.Migrate(); // Applies all migrations (code-first)
                context.Seed();
            }

            // Assert seeded data
            using (var context = new AppDbContext(options))
            {
                Assert.True(context.Items.Any());
                Assert.True(context.Monsters.Any());
                Assert.True(context.Characters.Any());
                Assert.True(context.MonsterItemDrops.Any());
                Assert.True(context.CharacterItems.Any());
            }
        }*/
    }

}