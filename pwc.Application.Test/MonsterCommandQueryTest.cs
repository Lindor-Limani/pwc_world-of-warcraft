using AutoMapper;
using Moq;
using pwc.Application.CQRS.Commands.Monster;
using pwc.Application.CQRS.Queries.Monster;
using pwc.Domain.DTOs;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace pwc.Application.Test
{
    public class MonsterCommandQueryTest
    {
        private readonly Mock<IMonsterRepository> _monsterRepoMock;
        private readonly Mock<IItemRepository> _itemRepoMock;
        private readonly Mock<IMapper> _mapperMock;

        public MonsterCommandQueryTest()
        {
            _monsterRepoMock = new Mock<IMonsterRepository>();
            _itemRepoMock = new Mock<IItemRepository>();
            _mapperMock = new Mock<IMapper>();
        }

        // --- CreateMonsterCommandHandler ---
        [Fact]
        public async Task CreateMonsterCommandHandler_ReturnsMonsterDto_WhenSuccess()
        {
            var command = new CreateMonsterCommand("Goblin", 10, 2);
            var monster = new Monster { Id = 1, Name = "Goblin", Health = 10, Damage = 2 };
            var monsterDto = new MonsterDto { Id = 1, Name = "Goblin", Health = 10, Damage = 2 };

            _mapperMock.Setup(m => m.Map<Monster>(command)).Returns(monster);
            _monsterRepoMock.Setup(r => r.AddAsync(monster)).ReturnsAsync(monster);
            _mapperMock.Setup(m => m.Map<MonsterDto>(monster)).Returns(monsterDto);

            var handler = new CreateMonsterCommandHandler(_monsterRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(monsterDto.Id, result.Id);
            Assert.Equal(monsterDto.Name, result.Name);
        }

        [Fact]
        public async Task CreateMonsterCommandHandler_ThrowsApplicationException_OnException()
        {
            var command = new CreateMonsterCommand("Goblin", 10, 2);
            _mapperMock.Setup(m => m.Map<Monster>(command)).Throws(new Exception("Mapping failed"));

            var handler = new CreateMonsterCommandHandler(_monsterRepoMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                handler.Handle(command, CancellationToken.None));
            Assert.Contains("Error creating monster", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("Mapping failed", ex.InnerException.Message);
        }

        // --- DeleteMonsterCommandHandler ---
        [Fact]
        public async Task DeleteMonsterCommandHandler_ReturnsTrue_WhenSuccess()
        {
            _monsterRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var handler = new DeleteMonsterCommandHandler(_monsterRepoMock.Object);

            var result = await handler.Handle(new DeleteMonsterCommand(1), CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteMonsterCommandHandler_ThrowsApplicationException_OnException()
        {
            _monsterRepoMock.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Exception("DB error"));

            var handler = new DeleteMonsterCommandHandler(_monsterRepoMock.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                handler.Handle(new DeleteMonsterCommand(1), CancellationToken.None));
            Assert.Contains("Error deleting monster", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("DB error", ex.InnerException.Message);
        }

        // --- UpdateMonsterCommandHandler ---
        [Fact]
        public async Task UpdateMonsterCommandHandler_ReturnsMonsterDto_WhenSuccess()
        {
            var monster = new Monster { Id = 1, Name = "Old", Health = 5, Damage = 1, MonsterItemDrops = new List<MonsterItemDrop>() };
            var updatedMonster = new Monster { Id = 1, Name = "New", Health = 20, Damage = 5, MonsterItemDrops = new List<MonsterItemDrop>() };
            var monsterDto = new MonsterDto { Id = 1, Name = "New", Health = 20, Damage = 5 };
            var command = new UpdateMonsterCommand(1, "New", 20, 5, new List<int> { 2 });
            var item = new Item { Id = 2, Name = "Sword" };

            _monsterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(monster);
            _itemRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(item);
            _monsterRepoMock.Setup(r => r.UpdateAsync(monster)).ReturnsAsync(updatedMonster);
            _mapperMock.Setup(m => m.Map<MonsterDto>(monster)).Returns(monsterDto);

            var handler = new UpdateMonsterCommandHandler(_monsterRepoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("New", result.Name);
        }

        [Fact]
        public async Task UpdateMonsterCommandHandler_ThrowsKeyNotFound_WhenMonsterNotFound()
        {
            _monsterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Monster)null);
            var handler = new UpdateMonsterCommandHandler(_monsterRepoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new UpdateMonsterCommand(1, "X", 1, 1, new List<int>()), CancellationToken.None));
        }

        [Fact]
        public async Task UpdateMonsterCommandHandler_ThrowsKeyNotFound_WhenItemNotFound()
        {
            var monster = new Monster { Id = 1, Name = "Old", MonsterItemDrops = new List<MonsterItemDrop>() };
            _monsterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(monster);
            _itemRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Item)null);
            var handler = new UpdateMonsterCommandHandler(_monsterRepoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new UpdateMonsterCommand(1, "X", 1, 1, new List<int> { 2 }), CancellationToken.None));
        }

        // --- GetMonsterByIdQueryHandler ---
        [Fact]
        public async Task GetMonsterByIdQueryHandler_ReturnsMonsterDto_WhenFound()
        {
            var monster = new Monster { Id = 1, Name = "Goblin" };
            var monsterDto = new MonsterDto { Id = 1, Name = "Goblin" };
            _monsterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(monster);
            _mapperMock.Setup(m => m.Map<MonsterDto>(monster)).Returns(monsterDto);

            var handler = new GetMonsterByIdQueryHandler(_monsterRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetMonsterByIdQuery(1), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetMonsterByIdQueryHandler_ThrowsKeyNotFound_WhenNotFound()
        {
            _monsterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Monster)null);

            var handler = new GetMonsterByIdQueryHandler(_monsterRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new GetMonsterByIdQuery(1), CancellationToken.None));
        }

        // --- GetAllMonstersQueryHandler ---
        [Fact]
        public async Task GetAllMonstersQueryHandler_ReturnsMonsterDtos()
        {
            var monsters = new List<Monster> { new Monster { Id = 1, Name = "Goblin" } };
            var monsterDtos = new List<MonsterDto> { new MonsterDto { Id = 1, Name = "Goblin" } };
            _monsterRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(monsters);
            _mapperMock.Setup(m => m.Map<List<MonsterDto>>(monsters)).Returns(monsterDtos);

            var handler = new GetAllMonstersQueryHandler(_monsterRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetAllMonsterQuery(), CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Goblin", result.First().Name);
        }

        // --- GetAllMonsterByNameQueryHandler ---
        [Fact]
        public async Task GetAllMonsterByNameQueryHandler_ReturnsMonsterDtos_WhenFound()
        {
            var monsters = new List<Monster> { new Monster { Id = 1, Name = "Goblin" } };
            var monsterDtos = new List<MonsterDto> { new MonsterDto { Id = 1, Name = "Goblin" } };
            _monsterRepoMock.Setup(r => r.GetByNameAsync("Goblin")).ReturnsAsync(monsters);
            _mapperMock.Setup(m => m.Map<List<MonsterDto>>(monsters)).Returns(monsterDtos);

            var handler = new GetAllMonsterByNameQueryHandler(_monsterRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetAllMonsterByNameQuery("Goblin"), CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Goblin", result.First().Name);
        }

        [Fact]
        public async Task GetAllMonsterByNameQueryHandler_ThrowsKeyNotFound_WhenNotFound()
        {
            _monsterRepoMock.Setup(r => r.GetByNameAsync("Goblin")).ReturnsAsync((IEnumerable<Monster>)null);

            var handler = new GetAllMonsterByNameQueryHandler(_monsterRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new GetAllMonsterByNameQuery("Goblin"), CancellationToken.None));
        }

        // --- AddDropToMonserCommandHandler ---
        [Fact]
        public async Task AddDropToMonserCommandHandler_ReturnsMonsterDto_WhenSuccess()
        {
            var command = new AddDropToMonsterCommand(1, 2, 0.5);
            var item = new Item { Id = 2, Name = "Sword" };
            var monsterE = new Monster { Id = 1, Name = "Goblin" };
            var monster = new Monster { Id = 1, Name = "Goblin" };
            var monsterDto = new MonsterDto { Id = 1, Name = "Goblin" };

            _itemRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(item);
            _monsterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(monsterE);
            _monsterRepoMock.Setup(r => r.AddItemDropAsync(1, 2, 0.5)).ReturnsAsync(monster);
            _mapperMock.Setup(m => m.Map<MonsterDto>(monster)).Returns(monsterDto);

            var handler = new AddDropToMonserCommandHandler(_monsterRepoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AddDropToMonserCommandHandler_ThrowsKeyNotFound_WhenItemNotFound()
        {
            var command = new AddDropToMonsterCommand(1, 2, 0.5);
            _itemRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Item)null);

            var handler = new AddDropToMonserCommandHandler(_monsterRepoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddDropToMonserCommandHandler_ThrowsKeyNotFound_WhenMonsterNotFound()
        {
            var command = new AddDropToMonsterCommand(1, 2, 0.5);
            var item = new Item { Id = 2, Name = "Sword" };
            _itemRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(item);
            _monsterRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Monster)null);

            var handler = new AddDropToMonserCommandHandler(_monsterRepoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}
