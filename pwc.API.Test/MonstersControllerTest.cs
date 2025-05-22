using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using pwc.API.Controllers;
using pwc.Application.CQRS.Commands.Monster;
using pwc.Application.CQRS.Queries.Monster;
using pwc.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.API.Test
{
    public class MonstersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MonstersController _controller;

        public MonstersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new MonstersController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreateMonster_ReturnsCreatedAtAction_WhenValid()
        {
            var command = new CreateMonsterCommand("Goblin", 10, 2);
            var monsterDto = new MonsterDto { Id = 1, Name = "Goblin", Health = 10, Damage = 2 };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(monsterDto);

            var result = await _controller.CreateMonster(command);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(monsterDto, created.Value);
        }

        [Fact]
        public async Task CreateMonster_ReturnsBadRequest_OnValidationException()
        {
            var command = new CreateMonsterCommand("Goblin", 10, 2);
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new ValidationException("Invalid"));

            var result = await _controller.CreateMonster(command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task CreateMonster_ReturnsValidationProblem_WhenModelStateInvalid()
        {
            var command = new CreateMonsterCommand("Goblin", 10, 2);
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.CreateMonster(command);

            Assert.IsType<ObjectResult>(result); // ValidationProblem returns ObjectResult
        }

        [Fact]
        public async Task AddDropToMonster_ReturnsOk_WhenValid()
        {
            var command = new AddDropToMonsterCommand(1, 2, 0.5);
            var monsterDto = new MonsterDto { Id = 1, Name = "Goblin" };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(monsterDto);

            var result = await _controller.AddDropToMonster(1, command);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(monsterDto, ok.Value);
        }

        [Fact]
        public async Task AddDropToMonster_ReturnsBadRequest_WhenIdMismatch()
        {
            var command = new AddDropToMonsterCommand(2, 2, 0.5);

            var result = await _controller.AddDropToMonster(1, command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Route monsterId and command MonsterId do not match.", badRequest.Value);
        }

        [Fact]
        public async Task AddDropToMonster_ReturnsNotFound_OnKeyNotFoundException()
        {
            var command = new AddDropToMonsterCommand(1, 2, 0.5);
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.AddDropToMonster(1, command);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task AddDropToMonster_ReturnsBadRequest_OnInvalidOperationException()
        {
            var command = new AddDropToMonsterCommand(1, 2, 0.5);
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new InvalidOperationException("Invalid"));

            var result = await _controller.AddDropToMonster(1, command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task AddDropToMonster_ReturnsBadRequest_OnArgumentOutOfRangeException()
        {
            var command = new AddDropToMonsterCommand(1, 2, 0.5);
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new ArgumentOutOfRangeException("dropChance", "Out of range"));

            var result = await _controller.AddDropToMonster(1, command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Out of range", badRequest.Value.ToString());
        }

        [Fact]
        public async Task UpdateMonster_ReturnsOk_WhenValid()
        {
            var command = new UpdateMonsterCommand(1, "Goblin", 10, 2, new List<int>());
            var monsterDto = new MonsterDto { Id = 1, Name = "Goblin" };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(monsterDto);

            var result = await _controller.UpdateMonster(1, command);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(monsterDto, ok.Value);
        }

        [Fact]
        public async Task UpdateMonster_ReturnsBadRequest_WhenIdMismatch()
        {
            var command = new UpdateMonsterCommand(2, "Goblin", 10, 2, new List<int>());

            var result = await _controller.UpdateMonster(1, command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The route ID and command ID do not match.", badRequest.Value);
        }

        [Fact]
        public async Task UpdateMonster_ReturnsValidationProblem_WhenModelStateInvalid()
        {
            var command = new UpdateMonsterCommand(1, "Goblin", 10, 2, new List<int>());
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.UpdateMonster(1, command);

            Assert.IsType<ObjectResult>(result); // ValidationProblem returns ObjectResult
        }

        [Fact]
        public async Task UpdateMonster_ReturnsNotFound_OnKeyNotFoundException()
        {
            var command = new UpdateMonsterCommand(1, "Goblin", 10, 2, new List<int>());
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.UpdateMonster(1, command);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task UpdateMonster_ReturnsBadRequest_OnInvalidOperationException()
        {
            var command = new UpdateMonsterCommand(1, "Goblin", 10, 2, new List<int>());
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new InvalidOperationException("Invalid"));

            var result = await _controller.UpdateMonster(1, command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task DeleteMonster_ReturnsNoContent_WhenSuccess()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMonsterCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.DeleteMonster(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteMonster_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMonsterCommand>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.DeleteMonster(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task DeleteMonster_ReturnsBadRequest_OnInvalidOperationException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMonsterCommand>(), default)).ThrowsAsync(new InvalidOperationException("Invalid"));

            var result = await _controller.DeleteMonster(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task GetMonsterById_ReturnsOk_WhenFound()
        {
            var monsterDto = new MonsterDto { Id = 1, Name = "Goblin" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMonsterByIdQuery>(), default)).ReturnsAsync(monsterDto);

            var result = await _controller.GetMonsterById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(monsterDto, ok.Value);
        }

        [Fact]
        public async Task GetMonsterById_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMonsterByIdQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetMonsterById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task GetAllMonster_ReturnsOk_WhenFound()
        {
            var monsters = new List<MonsterDto> { new MonsterDto { Id = 1, Name = "Goblin" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllMonsterQuery>(), default)).ReturnsAsync(monsters);

            var result = await _controller.GetAllMonster();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(monsters, ok.Value);
        }

        [Fact]
        public async Task GetAllMonster_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllMonsterQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetAllMonster();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task GetMonsterByName_ReturnsOk_WhenFound()
        {
            var monsters = new List<MonsterDto> { new MonsterDto { Id = 1, Name = "Goblin" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllMonsterByNameQuery>(), default)).ReturnsAsync(monsters);

            var result = await _controller.GetMonsterByName("Goblin");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(monsters, ok.Value);
        }

        [Fact]
        public async Task GetMonsterByName_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllMonsterByNameQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetMonsterByName("Goblin");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }
    }
}
