using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using pwc.API.Controllers;
using pwc.Application.CQRS.Commands.Charakter;
using pwc.Application.CQRS.Queries.Charakter;
using pwc.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.API.Test
{
    public class CharaktersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CharaktersController _controller;

        public CharaktersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CharaktersController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreateCharakter_ReturnsCreatedAtAction_WhenValid()
        {
            var command = new CreateCharakterCommand("Hero");
            var charakterDto = new CharakterDto { Id = 1, Name = "Hero" };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(charakterDto);

            var result = await _controller.CreateCharakter(command);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(charakterDto, created.Value);
        }

        [Fact]
        public async Task CreateCharakter_ReturnsBadRequest_OnValidationException()
        {
            var command = new CreateCharakterCommand("Hero");
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new ValidationException("Invalid"));

            var result = await _controller.CreateCharakter(command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task CreateCharakter_ReturnsValidationProblem_WhenModelStateInvalid()
        {
            var command = new CreateCharakterCommand("Hero");
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.CreateCharakter(command);

            Assert.IsType<ObjectResult>(result); // ValidationProblem returns ObjectResult
        }

        [Fact]
        public async Task EquipItemToCharakter_ReturnsOk_WhenValid()
        {
            var charakterDto = new CharakterDto { Id = 1, Name = "Hero" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<EquipItemToCharakterCommand>(), default)).ReturnsAsync(charakterDto);

            var result = await _controller.EquipItemToCharakter(1, 2);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(charakterDto, ok.Value);
        }

        [Fact]
        public async Task EquipItemToCharakter_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<EquipItemToCharakterCommand>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.EquipItemToCharakter(1, 2);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task UpdateCharakter_ReturnsOk_WhenValid()
        {
            var command = new UpdateCharakterCommand { Id = 1, Name = "Hero" };
            var charakterDto = new CharakterDto { Id = 1, Name = "Hero" };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(charakterDto);

            var result = await _controller.UpdateCharakter(1, command);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(charakterDto, ok.Value);
        }

        [Fact]
        public async Task UpdateCharakter_ReturnsBadRequest_WhenIdMismatch()
        {
            var command = new UpdateCharakterCommand { Id = 2, Name = "Hero" };

            var result = await _controller.UpdateCharakter(1, command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The route ID and command ID do not match.", badRequest.Value);
        }

        [Fact]
        public async Task UpdateCharakter_ReturnsValidationProblem_WhenModelStateInvalid()
        {
            var command = new UpdateCharakterCommand { Id = 1, Name = "Hero" };
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.UpdateCharakter(1, command);

            Assert.IsType<ObjectResult>(result); // ValidationProblem returns ObjectResult
        }

        [Fact]
        public async Task UpdateCharakter_ReturnsNotFound_OnKeyNotFoundException()
        {
            var command = new UpdateCharakterCommand { Id = 1, Name = "Hero" };
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.UpdateCharakter(1, command);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task UpdateCharakter_ReturnsBadRequest_OnInvalidOperationException()
        {
            var command = new UpdateCharakterCommand { Id = 1, Name = "Hero" };
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new InvalidOperationException("Invalid"));

            var result = await _controller.UpdateCharakter(1, command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task DeleteCharakter_ReturnsNoContent_WhenSuccess()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCharakterCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.DeleteCharakter(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCharakter_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCharakterCommand>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.DeleteCharakter(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task DeleteCharakter_ReturnsBadRequest_OnInvalidOperationException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCharakterCommand>(), default)).ThrowsAsync(new InvalidOperationException("Invalid"));

            var result = await _controller.DeleteCharakter(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task GetCharakterById_ReturnsOk_WhenFound()
        {
            var charakterDto = new CharakterDto { Id = 1, Name = "Hero" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCharakterByIdQuery>(), default)).ReturnsAsync(charakterDto);

            var result = await _controller.GetCharakterById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(charakterDto, ok.Value);
        }

        [Fact]
        public async Task GetCharakterById_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCharakterByIdQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetCharakterById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task GetCharakterByName_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCharakterByNameQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetCharakterByName("Hero");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task GetAllCharakters_ReturnsOk_WhenFound()
        {
            var charakterDtos = new List<CharakterDto> { new CharakterDto { Id = 1, Name = "Hero" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllCharakterQuery>(), default)).ReturnsAsync(charakterDtos);

            var result = await _controller.GetAllCharakters();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(charakterDtos, ok.Value);
        }

        [Fact]
        public async Task GetAllCharakters_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllCharakterQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetAllCharakters();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }
    }
}
