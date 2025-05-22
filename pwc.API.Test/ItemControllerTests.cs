using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using pwc.API.Controllers;
using pwc.Application.CQRS.Commands.Items;
using pwc.Application.CQRS.Queries.Item;
using pwc.Domain.DTOs;
using pwc.Domain.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.API.Test
{
    public class ItemControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ItemsController _controller;

        public ItemControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ItemsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreateItem_ReturnsCreatedAtAction_WhenValid()
        {
            var command = new CreateItemCommand("Sword", 1, 2, 3, ItemCategory.Waffe);
            var itemDto = new ItemDto { Id = 1, Name = "Sword", Category = ItemCategory.Waffe };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(itemDto);

            var result = await _controller.CreateItem(command);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(itemDto, created.Value);
        }

        [Fact]
        public async Task CreateItem_ReturnsBadRequest_OnValidationException()
        {
            var command = new CreateItemCommand("Sword", 1, 2, 3, ItemCategory.Waffe);
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new ValidationException("Invalid"));

            var result = await _controller.CreateItem(command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", badRequest.Value);
        }

        [Fact]
        public async Task UpdateItem_ReturnsOk_WhenValid()
        {
            var command = new UpdateItemCommand(1, "Sword", 1, 2, 3, ItemCategory.Waffe);
            var itemDto = new ItemDto { Id = 1, Name = "Sword", Category = ItemCategory.Waffe };
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync(itemDto);

            var result = await _controller.UpdateItem(1, command);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(itemDto, ok.Value);
        }

        [Fact]
        public async Task UpdateItem_ReturnsBadRequest_WhenIdMismatch()
        {
            var command = new UpdateItemCommand(2, "Sword", 1, 2, 3, ItemCategory.Waffe);

            var result = await _controller.UpdateItem(1, command);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Route id and command id do not match.", badRequest.Value);
        }

        [Fact]
        public async Task UpdateItem_ReturnsNotFound_OnKeyNotFoundException()
        {
            var command = new UpdateItemCommand(1, "Sword", 1, 2, 3, ItemCategory.Waffe);
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.UpdateItem(1, command);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task DeleteItem_ReturnsNoContent_WhenSuccess()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteItemCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.DeleteItem(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteItem_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteItemCommand>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.DeleteItem(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task GetItemById_ReturnsOk_WhenFound()
        {
            var itemDto = new ItemDto { Id = 1, Name = "Sword", Category = ItemCategory.Waffe };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemByIdQuery>(), default)).ReturnsAsync(itemDto);

            var result = await _controller.GetItemById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(itemDto, ok.Value);
        }

        [Fact]
        public async Task GetItemById_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemByIdQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetItemById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task GetAllItems_ReturnsOk_WhenFound()
        {
            var items = new List<ItemDto> { new ItemDto { Id = 1, Name = "Sword", Category = ItemCategory.Waffe } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllItemsQuery>(), default)).ReturnsAsync(items);

            var result = await _controller.GetAllItems();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetAllItems_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllItemsQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetAllItems();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task GetItemsByCategory_ReturnsOk_WhenFound()
        {
            var items = new List<ItemDto> { new ItemDto { Id = 1, Name = "Sword", Category = ItemCategory.Waffe } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemByCategoryQuery>(), default)).ReturnsAsync(items);

            var result = await _controller.GetItemsByCategory(ItemCategory.Waffe);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetItemsByCharakterId_ReturnsOk_WhenFound()
        {
            var items = new List<ItemDto> { new ItemDto { Id = 1, Name = "Sword", Category = ItemCategory.Waffe } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemByCharakterIdQuery>(), default)).ReturnsAsync(items);

            var result = await _controller.GetItemsByCharakterId(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetItemsByCharakterId_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemByCharakterIdQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetItemsByCharakterId(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }

        [Fact]
        public async Task GetItemsByName_ReturnsOk_WhenFound()
        {
            var items = new List<ItemDto> { new ItemDto { Id = 1, Name = "Sword", Category = ItemCategory.Waffe } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemByNameQuery>(), default)).ReturnsAsync(items);

            var result = await _controller.GetItemsByName("Sword");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(items, ok.Value);
        }

        [Fact]
        public async Task GetItemsByName_ReturnsNotFound_OnKeyNotFoundException()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetItemByNameQuery>(), default)).ThrowsAsync(new KeyNotFoundException("Not found"));

            var result = await _controller.GetItemsByName("Sword");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFound.Value);
        }
    }
}
