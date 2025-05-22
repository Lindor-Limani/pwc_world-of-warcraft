using Microsoft.EntityFrameworkCore;
using pwc.Domain.Exceptions;
using pwc.Domain.Model.Enum;
using pwc.Domain.Model;
using pwc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Repository.Test
{
    public class CharakterRepositoryTest
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new AppDbContext(options);
            return context;
        }

        private Charakter CreateCharakter(int id = 1, string name = "Hero")
        {
            return new Charakter
            {
                Id = id,
                Name = name,
                CharakterItems = new List<CharakterItem>()
            };
        }

        private Item CreateItem(int id = 1, string name = "Sword", ItemCategory category = ItemCategory.Waffe)
        {
            return new Item
            {
                Id = id,
                Name = name,
                Geschicklichkeit = 1,
                Staerke = 2,
                Ausdauer = 3,
                Category = category,
                CharakterItems = new List<CharakterItem>()
            };
        }

        [Fact]
        public async Task AddAsync_AddsCharakterAndReturnsEntity()
        {
            var context = GetDbContext(nameof(AddAsync_AddsCharakterAndReturnsEntity));
            var repo = new CharakterRepository(context);
            var charakter = CreateCharakter();

            var result = await repo.AddAsync(charakter);

            Assert.NotNull(result);
            Assert.Equal(charakter.Name, result.Name);
            Assert.Equal(1, context.Characters.Count());
        }

        [Fact]
        public async Task DeleteAsync_RemovesCharakterAndReturnsTrue()
        {
            var context = GetDbContext(nameof(DeleteAsync_RemovesCharakterAndReturnsTrue));
            var charakter = CreateCharakter();
            context.Characters.Add(charakter);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            var result = await repo.DeleteAsync(charakter.Id);

            Assert.True(result);
            Assert.Empty(context.Characters);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenCharakterNotFound()
        {
            var context = GetDbContext(nameof(DeleteAsync_ReturnsFalse_WhenCharakterNotFound));
            var repo = new CharakterRepository(context);

            var result = await repo.DeleteAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task EquipItemToCharakter_EquipsItem_WhenValid()
        {
            var context = GetDbContext(nameof(EquipItemToCharakter_EquipsItem_WhenValid));
            var charakter = CreateCharakter(1, "Hero");
            var item = CreateItem(2, "Sword", ItemCategory.Waffe);
            context.Characters.Add(charakter);
            context.Items.Add(item);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            var result = await repo.EquipItemToCharakter(charakter.Id, item.Id);

            Assert.NotNull(result);
            Assert.Single(result.CharakterItems);
            Assert.Equal(item.Id, result.CharakterItems.First().ItemId);
        }

        [Fact]
        public async Task EquipItemToCharakter_Throws_WhenCharakterNotFound()
        {
            var context = GetDbContext(nameof(EquipItemToCharakter_Throws_WhenCharakterNotFound));
            var item = CreateItem(2, "Sword");
            context.Items.Add(item);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                repo.EquipItemToCharakter(999, item.Id));
        }

        [Fact]
        public async Task EquipItemToCharakter_Throws_WhenItemNotFound()
        {
            var context = GetDbContext(nameof(EquipItemToCharakter_Throws_WhenItemNotFound));
            var charakter = CreateCharakter(1, "Hero");
            context.Characters.Add(charakter);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                repo.EquipItemToCharakter(charakter.Id, 999));
        }

        [Fact]
        public async Task EquipItemToCharakter_Throws_WhenSameItemEquippedTwice()
        {
            var context = GetDbContext(nameof(EquipItemToCharakter_Throws_WhenSameItemEquippedTwice));
            var charakter = CreateCharakter(1, "Hero");
            var item = CreateItem(2, "Sword", ItemCategory.Waffe);
            var charakterItem = new CharakterItem { CharakterId = charakter.Id, ItemId = item.Id, Item = item };
            charakter.CharakterItems.Add(charakterItem);
            context.Characters.Add(charakter);
            context.Items.Add(item);
            context.CharacterItems.Add(charakterItem);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            await Assert.ThrowsAsync<SameEquippmentTwiceException>(() =>
                repo.EquipItemToCharakter(charakter.Id, item.Id));
        }

        [Fact]
        public async Task EquipItemToCharakter_Throws_WhenCategoryAlreadyEquipped()
        {
            var context = GetDbContext(nameof(EquipItemToCharakter_Throws_WhenCategoryAlreadyEquipped));
            var charakter = CreateCharakter(1, "Hero");
            var item1 = CreateItem(2, "Sword", ItemCategory.Waffe);
            var item2 = CreateItem(3, "Axe", ItemCategory.Waffe);
            var charakterItem = new CharakterItem { CharakterId = charakter.Id, ItemId = item1.Id, Item = item1 };
            charakter.CharakterItems.Add(charakterItem);
            context.Characters.Add(charakter);
            context.Items.AddRange(item1, item2);
            context.CharacterItems.Add(charakterItem);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            await Assert.ThrowsAsync<CategoryAlreadyEquippedException>(() =>
                repo.EquipItemToCharakter(charakter.Id, item2.Id));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCharakters()
        {
            var context = GetDbContext(nameof(GetAllAsync_ReturnsAllCharakters));
            context.Characters.AddRange(
                CreateCharakter(1, "Hero"),
                CreateCharakter(2, "Mage")
            );
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Hero");
            Assert.Contains(result, c => c.Name == "Mage");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCharakter_WhenExists()
        {
            var context = GetDbContext(nameof(GetByIdAsync_ReturnsCharakter_WhenExists));
            var charakter = CreateCharakter();
            context.Characters.Add(charakter);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            var result = await repo.GetByIdAsync(charakter.Id);

            Assert.NotNull(result);
            Assert.Equal(charakter.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var context = GetDbContext(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
            var repo = new CharakterRepository(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsMatchingCharakters()
        {
            var context = GetDbContext(nameof(GetByNameAsync_ReturnsMatchingCharakters));
            context.Characters.AddRange(
                CreateCharakter(1, "Hero"),
                CreateCharakter(2, "Hero"),
                CreateCharakter(3, "Mage")
            );
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            var result = await repo.GetByNameAsync("Hero");

            Assert.Equal(2, result.Count());
            Assert.All(result, c => Assert.Equal("Hero", c.Name));
        }

        [Fact]
        public async Task GetCharakterItemsAsync_ReturnsCharakterItems()
        {
            var context = GetDbContext(nameof(GetCharakterItemsAsync_ReturnsCharakterItems));
            var charakter = CreateCharakter(1, "Hero");
            var item = CreateItem(2, "Sword");
            var charakterItem = new CharakterItem { CharakterId = charakter.Id, ItemId = item.Id, Item = item };
            context.Characters.Add(charakter);
            context.Items.Add(item);
            context.CharacterItems.Add(charakterItem);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            var result = await repo.GetCharakterItemsAsync(charakter.Id);

            Assert.Single(result);
            Assert.Equal(item.Id, result.First().ItemId);
        }

        [Fact]
        public async Task GetEquippedItemsByCharakterIdAsync_ReturnsEquippedItems()
        {
            var context = GetDbContext(nameof(GetEquippedItemsByCharakterIdAsync_ReturnsEquippedItems));
            var charakter = CreateCharakter(1, "Hero");
            var item = CreateItem(2, "Sword");
            var charakterItem = new CharakterItem { CharakterId = charakter.Id, ItemId = item.Id, Item = item };
            context.Characters.Add(charakter);
            context.Items.Add(item);
            context.CharacterItems.Add(charakterItem);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            var result = await repo.GetEquippedItemsByCharakterIdAsync(charakter.Id);

            Assert.Single(result);
            Assert.Equal(item.Id, result.First().Id);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAndReturnsCharakter_WhenExists()
        {
            var context = GetDbContext(nameof(UpdateAsync_UpdatesAndReturnsCharakter_WhenExists));
            var charakter = CreateCharakter();
            context.Characters.Add(charakter);
            context.SaveChanges();
            var repo = new CharakterRepository(context);

            var updated = new Charakter
            {
                Id = charakter.Id,
                Name = "Mage"
            };

            var result = await repo.UpdateAsync(updated);

            Assert.NotNull(result);
            Assert.Equal("Mage", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenCharakterNotFound()
        {
            var context = GetDbContext(nameof(UpdateAsync_ReturnsNull_WhenCharakterNotFound));
            var repo = new CharakterRepository(context);

            var updated = new Charakter
            {
                Id = 999,
                Name = "Mage"
            };

            var result = await repo.UpdateAsync(updated);

            Assert.Null(result);
        }
    }
}
