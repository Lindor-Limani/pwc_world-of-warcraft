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


        public void SeedRPGGenre()
        {
            // Items: 3 swords, 3 armors, 4 rings
            var items = new List<Item>
    {
        // Swords
        new Item { Name = "Schwert des Lichts", Geschicklichkeit = 5, Staerke = 8, Ausdauer = 3, Category = ItemCategory.Waffe },
        new Item { Name = "Dunkelklinge", Geschicklichkeit = 4, Staerke = 9, Ausdauer = 2, Category = ItemCategory.Waffe },
        new Item { Name = "Drachenzahn", Geschicklichkeit = 6, Staerke = 7, Ausdauer = 4, Category = ItemCategory.Waffe },
        // Armors
        new Item { Name = "Plattenrüstung", Geschicklichkeit = 2, Staerke = 3, Ausdauer = 9, Category = ItemCategory.Ruestung },
        new Item { Name = "Lederwams", Geschicklichkeit = 6, Staerke = 2, Ausdauer = 6, Category = ItemCategory.Ruestung },
        new Item { Name = "Magierrobe", Geschicklichkeit = 5, Staerke = 1, Ausdauer = 7, Category = ItemCategory.Ruestung },
        // Rings
        new Item { Name = "Ring der Macht", Geschicklichkeit = 3, Staerke = 5, Ausdauer = 2, Category = ItemCategory.Accessoire },
        new Item { Name = "Ring der Geschwindigkeit", Geschicklichkeit = 8, Staerke = 2, Ausdauer = 1, Category = ItemCategory.Accessoire },
        new Item { Name = "Ring des Lebens", Geschicklichkeit = 2, Staerke = 2, Ausdauer = 8, Category = ItemCategory.Accessoire },
        new Item { Name = "Ring der Schatten", Geschicklichkeit = 7, Staerke = 3, Ausdauer = 2, Category = ItemCategory.Accessoire }
    };
            Items.AddRange(items);
            SaveChanges();

            // Monsters: 10 fantasy monsters
            var monsters = new List<Monster>
    {
        new Monster { Name = "Goblin", Health = 30, Damage = 5 },
        new Monster { Name = "Ork", Health = 50, Damage = 10 },
        new Monster { Name = "Dunkelwolf", Health = 40, Damage = 8 },
        new Monster { Name = "Schattengeist", Health = 25, Damage = 12 },
        new Monster { Name = "Feuerdrache", Health = 120, Damage = 25 },
        new Monster { Name = "Troll", Health = 80, Damage = 15 },
        new Monster { Name = "Skelettkrieger", Health = 35, Damage = 7 },
        new Monster { Name = "Hexe", Health = 28, Damage = 14 },
        new Monster { Name = "Riesenspinne", Health = 45, Damage = 9 },
        new Monster { Name = "Waldschrat", Health = 60, Damage = 11 }
    };
            Monsters.AddRange(monsters);
            SaveChanges();

            // Charakters: 3 example characters
            var charakters = new List<Charakter>
    {
        new Charakter { Name = "Arin" },
        new Charakter { Name = "Lysandra" },
        new Charakter { Name = "Borin" }
    };
            Characters.AddRange(charakters);
            SaveChanges();

            // MonsterItemDrops: Each monster drops 1-3 random items
            var rnd = new Random(1335);
            var monsterItemDrops = new List<MonsterItemDrop>();
            foreach (var monster in Monsters)
            {
                var dropItems = items.OrderBy(_ => Guid.NewGuid()).Take(rnd.Next(1, 4)).ToList();
                foreach (var item in dropItems)
                {
                    monsterItemDrops.Add(new MonsterItemDrop
                    {
                        MonsterId = monster.Id,
                        ItemId = item.Id,
                        DropChance = Math.Round(rnd.NextDouble() * 0.5 + 0.1, 2) // 0.1 - 0.6
                    });
                }
            }
            MonsterItemDrops.AddRange(monsterItemDrops);
            SaveChanges();

            // CharakterItems: Each character gets at most one item per category
            var charakterItems = new List<CharakterItem>();
            foreach (var charakter in Characters)
            {
                foreach (ItemCategory cat in Enum.GetValues(typeof(ItemCategory)))
                {
                    var item = items.Where(i => i.Category == cat).OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
                    if (item != null)
                    {
                        charakterItems.Add(new CharakterItem
                        {
                            CharakterId = charakter.Id,
                            ItemId = item.Id
                        });
                    }
                }
            }
            CharacterItems.AddRange(charakterItems);
            SaveChanges();
        }

    }
}
