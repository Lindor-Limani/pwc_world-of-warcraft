using MediatR;
using Microsoft.AspNetCore.Mvc;
using pwc.Application.CQRS.Commands.Charakter;
using pwc.Application.CQRS.Commands.Items;
using pwc.Application.CQRS.Queries.Item;
using pwc.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace pwc.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CharaktersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CharaktersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCharakter([FromBody] CreateCharakterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            try
            {
                var charakterDto = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetCharakterById), new { id = charakterDto.Id }, charakterDto);
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CharakterDto), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCharakterById(int id)
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
    }
}
