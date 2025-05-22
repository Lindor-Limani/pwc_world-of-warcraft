using Microsoft.EntityFrameworkCore;
using pwc.Domain.Model;
using pwc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Repository.Test
{
    public class MonsterRepositoryTest
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new AppDbContext(options);
            return context;
        }

        private Monster CreateMonster(int id = 1, string name = "Goblin", int health = 10, int damage = 2)
        {
            return new Monster
            {
                Id = id,
                Name = name,
                Health = health,
                Damage = damage,
                MonsterItemDrops = new List<MonsterItemDrop>()
            };
        }

        private Item CreateItem(int id = 1, string name = "Sword")
        {
            return new Item
            {
                Id = id,
                Name = name,
                Geschicklichkeit = 1,
                Staerke = 2,
                Ausdauer = 3,
                Category = pwc.Domain.Model.Enum.ItemCategory.Waffe
            };
        }

        [Fact]
        public async Task AddAsync_AddsMonsterAndReturnsEntity()
        {
            var context = GetDbContext(nameof(AddAsync_AddsMonsterAndReturnsEntity));
            var repo = new MonsterRepository(context);
            var monster = CreateMonster();

            var result = await repo.AddAsync(monster);

            Assert.NotNull(result);
            Assert.Equal(monster.Name, result.Name);
            Assert.Equal(1, context.Monsters.Count());
        }

        [Fact]
        public async Task DeleteAsync_RemovesMonsterAndReturnsTrue()
        {
            var context = GetDbContext(nameof(DeleteAsync_RemovesMonsterAndReturnsTrue));
            var monster = CreateMonster();
            context.Monsters.Add(monster);
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            var result = await repo.DeleteAsync(monster.Id);

            Assert.True(result);
            Assert.Empty(context.Monsters);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenMonsterNotFound()
        {
            var context = GetDbContext(nameof(DeleteAsync_ReturnsFalse_WhenMonsterNotFound));
            var repo = new MonsterRepository(context);

            var result = await repo.DeleteAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllMonsters()
        {
            var context = GetDbContext(nameof(GetAllAsync_ReturnsAllMonsters));
            context.Monsters.AddRange(
                CreateMonster(1, "Goblin"),
                CreateMonster(2, "Orc")
            );
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.Name == "Goblin");
            Assert.Contains(result, m => m.Name == "Orc");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMonster_WhenExists()
        {
            var context = GetDbContext(nameof(GetByIdAsync_ReturnsMonster_WhenExists));
            var monster = CreateMonster();
            context.Monsters.Add(monster);
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            var result = await repo.GetByIdAsync(monster.Id);

            Assert.NotNull(result);
            Assert.Equal(monster.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var context = GetDbContext(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
            var repo = new MonsterRepository(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsMatchingMonsters()
        {
            var context = GetDbContext(nameof(GetByNameAsync_ReturnsMatchingMonsters));
            context.Monsters.AddRange(
                CreateMonster(1, "Goblin"),
                CreateMonster(2, "Goblin"),
                CreateMonster(3, "Orc")
            );
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            var result = await repo.GetByNameAsync("Goblin");

            Assert.Equal(2, result.Count());
            Assert.All(result, m => Assert.Equal("Goblin", m.Name));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAndReturnsMonster_WhenExists()
        {
            var context = GetDbContext(nameof(UpdateAsync_UpdatesAndReturnsMonster_WhenExists));
            var monster = CreateMonster();
            context.Monsters.Add(monster);
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            var updated = new Monster
            {
                Id = monster.Id,
                Name = "Orc",
                Health = 20,
                Damage = 5
            };

            var result = await repo.UpdateAsync(updated);

            Assert.NotNull(result);
            Assert.Equal("Orc", result.Name);
            Assert.Equal(20, result.Health);
            Assert.Equal(5, result.Damage);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenMonsterNotFound()
        {
            var context = GetDbContext(nameof(UpdateAsync_ReturnsNull_WhenMonsterNotFound));
            var repo = new MonsterRepository(context);

            var updated = new Monster
            {
                Id = 999,
                Name = "Orc",
                Health = 20,
                Damage = 5
            };

            var result = await repo.UpdateAsync(updated);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddItemDropAsync_AddsDropAndReturnsMonster()
        {
            var context = GetDbContext(nameof(AddItemDropAsync_AddsDropAndReturnsMonster));
            var monster = CreateMonster(1, "Goblin");
            var item = CreateItem(2, "Sword");
            context.Monsters.Add(monster);
            context.Items.Add(item);
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            var result = await repo.AddItemDropAsync(monster.Id, item.Id, 0.5);

            Assert.NotNull(result);
            Assert.Equal(monster.Id, result.Id);
            Assert.Contains(result.MonsterItemDrops, mid => mid.ItemId == item.Id && Math.Abs(mid.DropChance - 0.5) < 0.0001);
        }

        [Fact]
        public async Task AddItemDropAsync_ThrowsArgumentOutOfRangeException_WhenDropChanceInvalid()
        {
            var context = GetDbContext(nameof(AddItemDropAsync_ThrowsArgumentOutOfRangeException_WhenDropChanceInvalid));
            var monster = CreateMonster(1, "Goblin");
            var item = CreateItem(2, "Sword");
            context.Monsters.Add(monster);
            context.Items.Add(item);
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                repo.AddItemDropAsync(monster.Id, item.Id, -0.1));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                repo.AddItemDropAsync(monster.Id, item.Id, 1.1));
        }

        [Fact]
        public async Task AddItemDropAsync_ThrowsKeyNotFoundException_WhenMonsterNotFound()
        {
            var context = GetDbContext(nameof(AddItemDropAsync_ThrowsKeyNotFoundException_WhenMonsterNotFound));
            var item = CreateItem(2, "Sword");
            context.Items.Add(item);
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                repo.AddItemDropAsync(999, item.Id, 0.5));
        }

        [Fact]
        public async Task AddItemDropAsync_ThrowsKeyNotFoundException_WhenItemNotFound()
        {
            var context = GetDbContext(nameof(AddItemDropAsync_ThrowsKeyNotFoundException_WhenItemNotFound));
            var monster = CreateMonster(1, "Goblin");
            context.Monsters.Add(monster);
            context.SaveChanges();
            var repo = new MonsterRepository(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                repo.AddItemDropAsync(monster.Id, 999, 0.5));
        }
    }
}
