using Microsoft.EntityFrameworkCore;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model.Enum;
using pwc.Domain.Model;
using pwc.Infrastructure;
using Microsoft.EntityFrameworkCore.InMemory;

namespace pwc.Repository.Test
{
    public class ItemRepositoryTest
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new AppDbContext(options);
            return context;
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
                Category = category
            };
        }

        [Fact]
        public async Task AddAsync_AddsItemAndReturnsEntity()
        {
            var context = GetDbContext(nameof(AddAsync_AddsItemAndReturnsEntity));
            var repo = new ItemRepository(context);
            var item = CreateItem();

            var result = await repo.AddAsync(item);

            Assert.NotNull(result);
            Assert.Equal(item.Name, result.Name);
            Assert.Equal(1, context.Items.Count());
        }

        [Fact]
        public async Task DeleteAsync_RemovesItemAndReturnsTrue()
        {
            var context = GetDbContext(nameof(DeleteAsync_RemovesItemAndReturnsTrue));
            var item = CreateItem();
            context.Items.Add(item);
            context.SaveChanges();
            var repo = new ItemRepository(context);

            var result = await repo.DeleteAsync(item.Id);

            Assert.True(result);
            Assert.Empty(context.Items);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenItemNotFound()
        {
            var context = GetDbContext(nameof(DeleteAsync_ReturnsFalse_WhenItemNotFound));
            var repo = new ItemRepository(context);

            var result = await repo.DeleteAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllItems()
        {
            var context = GetDbContext(nameof(GetAllAsync_ReturnsAllItems));
            context.Items.AddRange(
                CreateItem(1, "Sword"),
                CreateItem(2, "Shield", ItemCategory.Ruestung)
            );
            context.SaveChanges();
            var repo = new ItemRepository(context);

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, i => i.Name == "Sword");
            Assert.Contains(result, i => i.Name == "Shield");
        }

        [Fact]
        public async Task GetByCategoryAsync_ReturnsFilteredItems()
        {
            var context = GetDbContext(nameof(GetByCategoryAsync_ReturnsFilteredItems));
            context.Items.AddRange(
                CreateItem(1, "Sword", ItemCategory.Waffe),
                CreateItem(2, "Shield", ItemCategory.Ruestung)
            );
            context.SaveChanges();
            var repo = new ItemRepository(context);

            var result = await repo.GetByCategoryAsync(ItemCategory.Waffe);

            Assert.Single(result);
            Assert.Equal("Sword", result.First().Name);
        }

        [Fact]
        public async Task GetByCharacterIdAsync_ReturnsItemsForCharakter()
        {
            var context = GetDbContext(nameof(GetByCharacterIdAsync_ReturnsItemsForCharakter));
            var item1 = CreateItem(1, "Sword");
            var item2 = CreateItem(2, "Shield");
            var charakterItem = new CharakterItem { CharakterId = 10, ItemId = 1, Item = item1 };
            item1.CharakterItems = new List<CharakterItem> { charakterItem };
            item2.CharakterItems = new List<CharakterItem>();
            context.Items.AddRange(item1, item2);
            context.CharacterItems.Add(charakterItem);
            context.SaveChanges();
            var repo = new ItemRepository(context);

            var result = await repo.GetByCharacterIdAsync(10);

            Assert.Single(result);
            Assert.Equal("Sword", result.First().Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsItem_WhenExists()
        {
            var context = GetDbContext(nameof(GetByIdAsync_ReturnsItem_WhenExists));
            var item = CreateItem();
            context.Items.Add(item);
            context.SaveChanges();
            var repo = new ItemRepository(context);

            var result = await repo.GetByIdAsync(item.Id);

            Assert.NotNull(result);
            Assert.Equal(item.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var context = GetDbContext(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
            var repo = new ItemRepository(context);

            var result = await repo.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsMatchingItems()
        {
            var context = GetDbContext(nameof(GetByNameAsync_ReturnsMatchingItems));
            context.Items.AddRange(
                CreateItem(1, "Sword"),
                CreateItem(2, "Sword"),
                CreateItem(3, "Shield")
            );
            context.SaveChanges();
            var repo = new ItemRepository(context);

            var result = await repo.GetByNameAsync("Sword");

            Assert.Equal(2, result.Count());
            Assert.All(result, i => Assert.Equal("Sword", i.Name));
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAndReturnsItem_WhenExists()
        {
            var context = GetDbContext(nameof(UpdateAsync_UpdatesAndReturnsItem_WhenExists));
            var item = CreateItem();
            context.Items.Add(item);
            context.SaveChanges();
            var repo = new ItemRepository(context);

            var updated = new Item
            {
                Id = item.Id,
                Name = "Axe",
                Geschicklichkeit = 5,
                Staerke = 6,
                Ausdauer = 7,
                Category = ItemCategory.Waffe
            };

            var result = await repo.UpdateAsync(updated);

            Assert.NotNull(result);
            Assert.Equal("Axe", result.Name);
            Assert.Equal(5, result.Geschicklichkeit);
            Assert.Equal(6, result.Staerke);
            Assert.Equal(7, result.Ausdauer);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenItemNotFound()
        {
            var context = GetDbContext(nameof(UpdateAsync_ReturnsNull_WhenItemNotFound));
            var repo = new ItemRepository(context);

            var updated = new Item
            {
                Id = 999,
                Name = "Axe",
                Geschicklichkeit = 5,
                Staerke = 6,
                Ausdauer = 7,
                Category = ItemCategory.Waffe
            };

            var result = await repo.UpdateAsync(updated);

            Assert.Null(result);
        }
    }


}