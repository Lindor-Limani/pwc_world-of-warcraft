using Bogus;
using Microsoft.EntityFrameworkCore;
using pwc.Domain.Model;
using pwc.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items => Set<Item>();
        public DbSet<Monster> Monsters => Set<Monster>();
        public DbSet<Charakter> Characters => Set<Charakter>();
        public DbSet<CharakterItem> CharacterItems => Set<CharakterItem>();
        public DbSet<MonsterItemDrop> MonsterItemDrops => Set<MonsterItemDrop>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MonsterItemDrop>()
                .HasKey(mid => new { mid.MonsterId, mid.ItemId });

            modelBuilder.Entity<CharakterItem>()
                .HasKey(ci => new { ci.CharakterId, ci.ItemId });

            modelBuilder.Entity<MonsterItemDrop>()
                .HasOne(mid => mid.Monster)
                .WithMany(m => m.MonsterItemDrops)
                .HasForeignKey(mid => mid.MonsterId);

            modelBuilder.Entity<MonsterItemDrop>()
                .HasOne(mid => mid.Item)
                .WithMany(i => i.MonsterItemDrops)
                .HasForeignKey(mid => mid.ItemId);

            modelBuilder.Entity<CharakterItem>()
                .HasOne(ci => ci.Charakter)
                .WithMany(c => c.CharakterItems)
                .HasForeignKey(ci => ci.CharakterId);

            modelBuilder.Entity<CharakterItem>()
                .HasOne(ci => ci.Item)
                .WithMany(i => i.CharakterItems)
                .HasForeignKey(ci => ci.ItemId);
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(1335);

            var itemFaker = new Faker<Item>()
        .RuleFor(i => i.Name, f => f.Commerce.ProductName())
        .RuleFor(i => i.Geschicklichkeit, f => f.Random.Int(1, 10))
        .RuleFor(i => i.Staerke, f => f.Random.Int(1, 10))
        .RuleFor(i => i.Ausdauer, f => f.Random.Int(1, 10))
        .RuleFor(i => i.Category, f => f.PickRandom<ItemCategory>());
            var items = itemFaker.Generate(10);
            Items.AddRange(items);
            SaveChanges();

            var monsterFaker = new Faker<Monster>()
        .RuleFor(m => m.Name, f => f.Hacker.Noun() + " " + f.Random.Word())
        .RuleFor(m => m.Health, f => f.Random.Int(10, 200))
        .RuleFor(m => m.Damage, f => f.Random.Int(1, 50));
            var monsters = monsterFaker.Generate(5);
            Monsters.AddRange(monsters);
            SaveChanges();

            var charakterFaker = new Faker<Charakter>()
        .RuleFor(c => c.Name, f => f.Name.FirstName());
            var charakters = charakterFaker.Generate(3);
            Characters.AddRange(charakters);
            SaveChanges();

            // MonsterItemDrops (each monster drops 1-3 random items)
            var monsterItemDrops = new List<MonsterItemDrop>();
            foreach (var monster in Monsters)
            {
                var dropItems = items.OrderBy(_ => Guid.NewGuid()).Take(new Random().Next(1, 4)).ToList();
                foreach (var item in dropItems)
                {
                    monsterItemDrops.Add(new MonsterItemDrop
                    {
                        MonsterId = monster.Id,
                        ItemId = item.Id,
                        DropChance = new Random().NextDouble() * 0.5 + 0.1 // 0.1 - 0.6
                    });
                }
            }
            MonsterItemDrops.AddRange(monsterItemDrops);
            SaveChanges();

            var charakterItems = new List<CharakterItem>();
            foreach (var charakter in Characters)
            {
                var equipItems = items.OrderBy(_ => Guid.NewGuid()).Take(new Random().Next(1, 3)).ToList();
                foreach (var item in equipItems)
                {
                    charakterItems.Add(new CharakterItem
                    {
                        CharakterId = charakter.Id,
                        ItemId = item.Id
                    });
                }
            }
            CharacterItems.AddRange(charakterItems);

            SaveChanges();
        }
    }
}
