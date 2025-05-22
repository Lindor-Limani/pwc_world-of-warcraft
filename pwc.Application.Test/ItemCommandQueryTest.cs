using AutoMapper;
using Moq;
using pwc.Application.CQRS.Commands.Items;
using pwc.Application.CQRS.Queries.Item;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model;
using pwc.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace pwc.Application.Test
{
    public class ItemCommandQueryTest
    {
        private readonly Mock<IItemRepository> _itemRepoMock;
        private readonly Mock<ICharakterRepository> _charakterRepoMock;
        private readonly Mock<IMapper> _mapperMock;

        public ItemCommandQueryTest()
        {
            _itemRepoMock = new Mock<IItemRepository>();
            _charakterRepoMock = new Mock<ICharakterRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        // --- CreateItemHandler ---
        [Fact]
        public async Task CreateItemHandler_ReturnsItemDto_WhenSuccess()
        {
            var command = new CreateItemCommand("Sword", 1, 2, 3, ItemCategory.Waffe);
            var item = new Item { Id = 1, Name = "Sword" };
            var itemDto = new ItemDto { Id = 1, Name = "Sword" };

            _mapperMock.Setup(m => m.Map<Item>(command)).Returns(item);
            _itemRepoMock.Setup(r => r.AddAsync(item)).ReturnsAsync(item);
            _mapperMock.Setup(m => m.Map<ItemDto>(item)).Returns(itemDto);

            var handler = new CreateItemHandler(_itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(itemDto.Id, result.Id);
            Assert.Equal(itemDto.Name, result.Name);
        }

        [Fact]
        public async Task CreateItemHandler_ThrowsApplicationException_OnException()
        {
            var command = new CreateItemCommand("Sword", 1, 2, 3, ItemCategory.Waffe);
            _mapperMock.Setup(m => m.Map<Item>(command)).Throws(new Exception("Mapping failed"));

            var handler = new CreateItemHandler(_itemRepoMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                handler.Handle(command, CancellationToken.None));
            Assert.Contains("Error creating item", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("Mapping failed", ex.InnerException.Message);
        }

        // --- UpdateItemHandler ---
        [Fact]
        public async Task UpdateItemHandler_ReturnsItemDto_WhenSuccess()
        {
            var command = new UpdateItemCommand(1, "Sword", 1, 2, 3, ItemCategory.Waffe);
            var item = new Item { Id = 1, Name = "Old" };
            var updatedItem = new Item { Id = 1, Name = "Sword", Geschicklichkeit = 1, Staerke = 2, Ausdauer = 3, Category = ItemCategory.Waffe };
            var itemDto = new ItemDto { Id = 1, Name = "Sword" };

            _itemRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
            _itemRepoMock.Setup(r => r.UpdateAsync(item)).ReturnsAsync(updatedItem);
            _mapperMock.Setup(m => m.Map<ItemDto>(updatedItem)).Returns(itemDto);

            var handler = new UpdateItemHandler(_itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(itemDto.Id, result.Id);
            Assert.Equal(itemDto.Name, result.Name);
        }

        [Fact]
        public async Task UpdateItemHandler_ReturnsNull_WhenItemNotFound()
        {
            var command = new UpdateItemCommand(1, "Sword", 1, 2, 3, ItemCategory.Waffe);
            _itemRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Item)null);

            var handler = new UpdateItemHandler(_itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateItemHandler_ReturnsNull_WhenUpdateReturnsNull()
        {
            var command = new UpdateItemCommand(1, "Sword", 1, 2, 3, ItemCategory.Waffe);
            var item = new Item { Id = 1, Name = "Old" };
            _itemRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
            _itemRepoMock.Setup(r => r.UpdateAsync(item)).ReturnsAsync((Item)null);

            var handler = new UpdateItemHandler(_itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Null(result);
        }

        // --- DeleteItemHandler ---
        [Fact]
        public async Task DeleteItemHandler_ReturnsTrue_WhenSuccess()
        {
            var item = new Item { Id = 1, Name = "Sword" };
            _itemRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
            _itemRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var handler = new DeleteItemHandler(_itemRepoMock.Object);

            var result = await handler.Handle(new DeleteItemCommand(1), CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteItemHandler_ThrowsKeyNotFound_WhenItemNotFound()
        {
            _itemRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Item)null);

            var handler = new DeleteItemHandler(_itemRepoMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new DeleteItemCommand(1), CancellationToken.None));
        }

        [Fact]
        public async Task DeleteItemHandler_ThrowsApplicationException_OnException()
        {
            var item = new Item { Id = 1, Name = "Sword" };
            _itemRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
            _itemRepoMock.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Exception("DB error"));

            var handler = new DeleteItemHandler(_itemRepoMock.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                handler.Handle(new DeleteItemCommand(1), CancellationToken.None));
            Assert.Contains("Error deleting item", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("DB error", ex.InnerException.Message);
        }

        // --- GetItemByCategoryQueryHandler ---
        [Fact]
        public async Task GetItemByCategoryQueryHandler_ReturnsItemDtos()
        {
            var items = new List<Item> { new Item { Id = 1, Name = "Sword", Category = ItemCategory.Waffe } };
            var itemDtos = new List<ItemDto> { new ItemDto { Id = 1, Name = "Sword", Category = ItemCategory.Waffe } };
            _itemRepoMock.Setup(r => r.GetByCategoryAsync(ItemCategory.Waffe)).ReturnsAsync(items);
            _mapperMock.Setup(m => m.Map<List<ItemDto>>(items)).Returns(itemDtos);

            var handler = new GetItemByCategoryQueryHandler(_itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetItemByCategoryQuery(ItemCategory.Waffe), CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Sword", result.First().Name);
        }

        // --- GetItemByCharakterIdQueryHandler ---
        [Fact]
        public async Task GetItemByCharakterIdQueryHandler_ReturnsItemDtos_WhenFound()
        {
            var charakter = new Charakter { Id = 1, Name = "Hero" };
            var items = new List<Item> { new Item { Id = 1, Name = "Sword" } };
            var itemDtos = new List<ItemDto> { new ItemDto { Id = 1, Name = "Sword" } };

            _charakterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(charakter);
            _itemRepoMock.Setup(r => r.GetByCharacterIdAsync(1)).ReturnsAsync(items);
            _mapperMock.Setup(m => m.Map<List<ItemDto>>(items)).Returns(itemDtos);

            var handler = new GetItemByCharakterIdQueryHandler(_itemRepoMock.Object, _charakterRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetItemByCharakterIdQuery(1), CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Sword", result.First().Name);
        }

        [Fact]
        public async Task GetItemByCharakterIdQueryHandler_ThrowsKeyNotFound_WhenCharakterNotFound()
        {
            _charakterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Charakter)null);

            var handler = new GetItemByCharakterIdQueryHandler(_itemRepoMock.Object, _charakterRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new GetItemByCharakterIdQuery(1), CancellationToken.None));
        }

        [Fact]
        public async Task GetItemByCharakterIdQueryHandler_ThrowsKeyNotFound_WhenNoItems()
        {
            var charakter = new Charakter { Id = 1, Name = "Hero" };
            _charakterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(charakter);
            _itemRepoMock.Setup(r => r.GetByCharacterIdAsync(1)).ReturnsAsync(new List<Item>());

            var handler = new GetItemByCharakterIdQueryHandler(_itemRepoMock.Object, _charakterRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new GetItemByCharakterIdQuery(1), CancellationToken.None));
        }

        // --- GetItemByIdQueryHandler ---
        [Fact]
        public async Task GetItemByIdQueryHandler_ReturnsItemDto_WhenFound()
        {
            var item = new Item { Id = 1, Name = "Sword" };
            var itemDto = new ItemDto { Id = 1, Name = "Sword" };
            _itemRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);
            _mapperMock.Setup(m => m.Map<ItemDto>(item)).Returns(itemDto);

            var handler = new GetItemByIdQueryHandler(_itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetItemByIdQuery(1), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetItemByIdQueryHandler_ThrowsKeyNotFound_WhenNotFound()
        {
            _itemRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Item)null);

            var handler = new GetItemByIdQueryHandler(_itemRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new GetItemByIdQuery(1), CancellationToken.None));
        }

        // --- GetAllItemsQueryHandler ---
        [Fact]
        public async Task GetAllItemsQueryHandler_ReturnsItemDtos()
        {
            var items = new List<Item> { new Item { Id = 1, Name = "Sword" } };
            var itemDtos = new List<ItemDto> { new ItemDto { Id = 1, Name = "Sword" } };
            _itemRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
            _mapperMock.Setup(m => m.Map<List<ItemDto>>(items)).Returns(itemDtos);

            var handler = new GetAllItemsQueryHandler(_itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetAllItemsQuery(), CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Sword", result.First().Name);
        }
    }
}
