using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pwc.Application.CQRS.Commands.Items;
using pwc.Application.CQRS.Queries.Item;
using pwc.Domain.DTOs;
using pwc.Domain.Model.Enum;
using System.ComponentModel.DataAnnotations;

namespace pwc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        

        public ItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="command">The item creation data.</param>
        /// <returns>The created item.</returns>
        /// <response code="201">Returns the newly created item.</response>
        /// <response code="400">If the request is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            try
            {
                var itemDto = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetItemById), new { id = itemDto.Id }, itemDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the item");
            }
        }

        /// <summary>
        /// Updates an existing item.
        /// </summary>
        /// <param name="id">The item's ID.</param>
        /// <param name="command">The updated item data.</param>
        /// <returns>The updated item.</returns>
        /// <response code="200">Returns the updated item.</response>
        /// <response code="400">If the request is invalid or IDs do not match.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            // Ensure the route id matches the command id, to ensure Database integrity
            if (command.Id != id)
            {
                return BadRequest("Route id and command id do not match.");
            }

            try
            {
                var updatedItem = await _mediator.Send(command);
                return Ok(updatedItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the item");
            }
        }

        /// <summary>
        /// Deletes an item by ID.
        /// </summary>
        /// <param name="id">The item's ID.</param>
        /// <response code="204">If the item was deleted.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                await _mediator.Send(new DeleteItemCommand(id));
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the item");
            }
        }

        /// <summary>
        /// Gets an item by its unique ID.
        /// </summary>
        /// <param name="id">The item's ID.</param>
        /// <returns>The item DTO.</returns>
        /// <response code="200">Returns the item.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItemById(int id)
        {
            try
            {
                var query = new GetItemByIdQuery(id);
                var itemDto = await _mediator.Send(query);
                return Ok(itemDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the item");
            }
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>A list of all items.</returns>
        /// <response code="200">Returns the list of items.</response>
        /// <response code="404">If no items are found.</response>
        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                var query = new GetAllItemsQuery();
                var items = await _mediator.Send(query);
                return Ok(items);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the items");
            }
        }

        /// <summary>
        /// Gets all items by category.
        /// </summary>
        /// <param name="category">The item category (Waffe, Ruestung, Accessoire).</param>
        /// <returns>A list of items in the specified category.</returns>
        /// <response code="200">Returns the list of items.</response>
        /// <response code="400">If the category is invalid.</response>
        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetItemsByCategory(ItemCategory category)
        {
            try
            {
                var query = new GetItemByCategoryQuery(category);
                var items = await _mediator.Send(query);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the items by the category");
            }
        }

        /// <summary>
        /// Gets all items equipped by a character.
        /// </summary>
        /// <param name="charakterId">The character's ID.</param>
        /// <returns>A list of items equipped by the character.</returns>
        /// <response code="200">Returns the list of items.</response>
        /// <response code="404">If the character or items are not found.</response>
        [HttpGet("character/{charakterId}")]
        [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItemsByCharakterId(int charakterId)
        {
            try
            {
                var query = new GetItemByCharakterIdQuery(charakterId);
                var items = await _mediator.Send(query);
                return Ok(items);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the items by the charakter id");
            }
        }

        /// <summary>
        /// Gets all items by name (partial or full match).
        /// </summary>
        /// <param name="name">The item name to search for.</param>
        /// <returns>A list of matching items.</returns>
        /// <response code="200">Returns the list of items.</response>
        /// <response code="404">If no items are found.</response>
        [HttpGet("itemName/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItemsByName(string name)
        {
            try
            {
                var query = new GetItemByNameQuery(name);
                var item = await _mediator.Send(query);
                return Ok(item);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the item by name");
            }
        }
    } 
}
