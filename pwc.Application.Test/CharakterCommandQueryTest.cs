using AutoMapper;
using Moq;
using pwc.Application.CQRS.Commands.Charakter;
using pwc.Application.CQRS.Queries.Charakter;
using pwc.Domain.DTOs;
using pwc.Domain.Exceptions;
using pwc.Domain.Interface.Repo;
using pwc.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Application.Test
{
    public class CharakterCommandQueryTest
    {
        private readonly Mock<ICharakterRepository> _repoMock;
        private readonly Mock<IItemRepository> _itemRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateCharakterHandler _handler;

        public CharakterCommandQueryTest()
        {
            _repoMock = new Mock<ICharakterRepository>();
            _itemRepoMock = new Mock<IItemRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateCharakterHandler(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCharakterDto_WhenSuccess()
        {
            // Arrange
            var command = new CreateCharakterCommand("Hero");
            var charakter = new Charakter { Id = 1, Name = "Hero" };
            var charakterDto = new CharakterDto { Id = 1, Name = "Hero" };

            _mapperMock.Setup(m => m.Map<Charakter>(command)).Returns(charakter);
            _repoMock.Setup(r => r.AddAsync(charakter)).ReturnsAsync(charakter);
            _mapperMock.Setup(m => m.Map<CharakterDto>(charakter)).Returns(charakterDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(charakterDto.Id, result.Id);
            Assert.Equal(charakterDto.Name, result.Name);
        }

        [Fact]
        public async Task Handle_ThrowsApplicationException_OnException()
        {
            // Arrange
            var command = new CreateCharakterCommand("Hero");
            _mapperMock.Setup(m => m.Map<Charakter>(command)).Throws(new Exception("Mapping failed"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Contains("Error creating charakter", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("Mapping failed", ex.InnerException.Message);
        }


        // --- DeleteCharakterCommandHandler ---
        [Fact]
        public async Task DeleteCharakterCommandHandler_ReturnsTrue_WhenSuccess()
        {
            var handler = new DeleteCharakterCommandHandler(_repoMock.Object);
            _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await handler.Handle(new DeleteCharakterCommand(1), CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCharakterCommandHandler_ThrowsApplicationException_OnException()
        {
            var handler = new DeleteCharakterCommandHandler(_repoMock.Object);
            _repoMock.Setup(r => r.DeleteAsync(1)).ThrowsAsync(new Exception("DB error"));

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                handler.Handle(new DeleteCharakterCommand(1), CancellationToken.None));
            Assert.Contains("Error deleting charakter", ex.Message);
            Assert.NotNull(ex.InnerException);
            Assert.Equal("DB error", ex.InnerException.Message);
        }


        // --- UpdateCharakterCommandHandler ---
        [Fact]
        public async Task UpdateCharakterCommandHandler_ReturnsCharakterDto_WhenSuccess()
        {
            var charakter = new Charakter { Id = 1, Name = "Old" };
            var updatedCharakter = new Charakter { Id = 1, Name = "New", CharakterItems = new List<CharakterItem>() };
            var charakterDto = new CharakterDto { Id = 1, Name = "New" };
            var command = new UpdateCharakterCommand { Id = 1, Name = "New", EquippedItemIds = new List<int> { 2 } };
            var item = new Item { Id = 2, Name = "Sword" };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(charakter);
            _itemRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(item);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Charakter>())).ReturnsAsync(updatedCharakter);
            _mapperMock.Setup(m => m.Map<CharakterDto>(It.IsAny<Charakter>())).Returns(charakterDto);

            var handler = new UpdateCharakterCommandHandler(_repoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("New", result.Name);
        }

        [Fact]
        public async Task UpdateCharakterCommandHandler_ThrowsKeyNotFound_WhenCharakterNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Charakter)null);
            var handler = new UpdateCharakterCommandHandler(_repoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new UpdateCharakterCommand { Id = 1, Name = "X" }, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateCharakterCommandHandler_ThrowsKeyNotFound_WhenItemNotFound()
        {
            var charakter = new Charakter { Id = 1, Name = "Old" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(charakter);
            _itemRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Item)null);
            var handler = new UpdateCharakterCommandHandler(_repoMock.Object, _itemRepoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new UpdateCharakterCommand { Id = 1, Name = "X", EquippedItemIds = new List<int> { 2 } }, CancellationToken.None));
        }

        // --- EquipItemToCharakterCommandHandler ---
        [Fact]
        public async Task EquipItemToCharakterCommandHandler_ReturnsCharakterDto_WhenSuccess()
        {
            var charakter = new Charakter { Id = 1, Name = "Hero" };
            var charakterDto = new CharakterDto { Id = 1, Name = "Hero" };
            _repoMock.Setup(r => r.EquipItemToCharakter(1, 2)).ReturnsAsync(charakter);
            _mapperMock.Setup(m => m.Map<CharakterDto>(charakter)).Returns(charakterDto);

            var handler = new EquipItemToCharakterCommandHandler(_repoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new EquipItemToCharakterCommand(1, 2), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task EquipItemToCharakterCommandHandler_ThrowsSameEquippmentTwiceException()
        {
            _repoMock.Setup(r => r.EquipItemToCharakter(1, 2)).ThrowsAsync(new SameEquippmentTwiceException("Already equipped"));
            var handler = new EquipItemToCharakterCommandHandler(_repoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<SameEquippmentTwiceException>(() =>
                handler.Handle(new EquipItemToCharakterCommand(1, 2), CancellationToken.None));
        }

        [Fact]
        public async Task EquipItemToCharakterCommandHandler_ThrowsCategoryAlreadyEquippedException()
        {
            _repoMock.Setup(r => r.EquipItemToCharakter(1, 2)).ThrowsAsync(new CategoryAlreadyEquippedException("Category equipped"));
            var handler = new EquipItemToCharakterCommandHandler(_repoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<CategoryAlreadyEquippedException>(() =>
                handler.Handle(new EquipItemToCharakterCommand(1, 2), CancellationToken.None));
        }

        [Fact]
        public async Task EquipItemToCharakterCommandHandler_ThrowsKeyNotFoundException()
        {
            _repoMock.Setup(r => r.EquipItemToCharakter(1, 2)).ThrowsAsync(new KeyNotFoundException("Not found"));
            var handler = new EquipItemToCharakterCommandHandler(_repoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new EquipItemToCharakterCommand(1, 2), CancellationToken.None));
        }

        [Fact]
        public async Task EquipItemToCharakterCommandHandler_ThrowsInvalidOperationException()
        {
            _repoMock.Setup(r => r.EquipItemToCharakter(1, 2)).ThrowsAsync(new InvalidOperationException("Invalid op"));
            var handler = new EquipItemToCharakterCommandHandler(_repoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new EquipItemToCharakterCommand(1, 2), CancellationToken.None));
        }

        [Fact]
        public async Task EquipItemToCharakterCommandHandler_ThrowsApplicationException_OnOtherException()
        {
            _repoMock.Setup(r => r.EquipItemToCharakter(1, 2)).ThrowsAsync(new Exception("Other"));
            var handler = new EquipItemToCharakterCommandHandler(_repoMock.Object, _mapperMock.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                handler.Handle(new EquipItemToCharakterCommand(1, 2), CancellationToken.None));
            Assert.Contains("Error creating charakter", ex.Message);
        }

        // --- GetAllCharaktersQueryHandler ---
        [Fact]
        public async Task GetAllCharaktersQueryHandler_ReturnsCharakterDtos()
        {
            var charakters = new List<Charakter> { new Charakter { Id = 1, Name = "Hero" } };
            var charakterDtos = new List<CharakterDto> { new CharakterDto { Id = 1, Name = "Hero" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(charakters);
            _mapperMock.Setup(m => m.Map<List<CharakterDto>>(charakters)).Returns(charakterDtos);

            var handler = new GetAllCharaktersQueryHandler(_repoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetAllCharakterQuery(), CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Hero", result.First().Name);
        }

        // --- GetCharakterByIdQueryHandler ---
        [Fact]
        public async Task GetCharakterByIdQueryHandler_ReturnsCharakterDto_WhenFound()
        {
            var charakter = new Charakter { Id = 1, Name = "Hero" };
            var charakterDto = new CharakterDto { Id = 1, Name = "Hero" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(charakter);
            _mapperMock.Setup(m => m.Map<CharakterDto>(charakter)).Returns(charakterDto);

            var handler = new GetCharakterByIdQueryHandler(_repoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetCharakterByIdQuery(1), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetCharakterByIdQueryHandler_ThrowsKeyNotFound_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Charakter)null);
            var handler = new GetCharakterByIdQueryHandler(_repoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new GetCharakterByIdQuery(1), CancellationToken.None));
        }

        // --- GetCharakterByNameQueryHandler ---
        [Fact]
        public async Task GetCharakterByNameQueryHandler_ReturnsCharakterDtos_WhenFound()
        {
            var charakters = new List<Charakter> { new Charakter { Id = 1, Name = "Hero" } };
            var charakterDtos = new List<CharakterDto> { new CharakterDto { Id = 1, Name = "Hero" } };
            _repoMock.Setup(r => r.GetByNameAsync("Hero")).ReturnsAsync(charakters);
            _mapperMock.Setup(m => m.Map<List<CharakterDto>>(charakters)).Returns(charakterDtos);

            var handler = new GetCharakterByNameQueryHandler(_repoMock.Object, _mapperMock.Object);

            var result = await handler.Handle(new GetCharakterByNameQuery("Hero"), CancellationToken.None);

            Assert.Single(result);
            Assert.Equal("Hero", result.First().Name);
        }

        [Fact]
        public async Task GetCharakterByNameQueryHandler_ThrowsKeyNotFound_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetByNameAsync("Hero")).ReturnsAsync((IEnumerable<Charakter>)null);
            var handler = new GetCharakterByNameQueryHandler(_repoMock.Object, _mapperMock.Object);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(new GetCharakterByNameQuery("Hero"), CancellationToken.None));
        }
    }

}
