using MediatR;
using Microsoft.AspNetCore.Mvc;
using pwc.Application.CQRS.Commands.Charakter;
using pwc.Application.CQRS.Commands.Items;
using pwc.Application.CQRS.Queries.Charakter;
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
        [ProducesResponseType(typeof(CharakterDto), StatusCodes.Status201Created)]
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
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the charakter");
            }
        }

        
        [HttpPost("equip/{charakterID}/{itemID}")]
        [ProducesResponseType(typeof(CharakterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EquipItemToCharakter(int charakterID, int itemID)
        {
            try
            {
                var command = new EquipItemToCharakterCommand(charakterID, itemID);
                var charakterDto = await _mediator.Send(command);
                return Ok(charakterDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the charakters");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CharakterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCharakter(int id, [FromBody] UpdateCharakterCommand command)
        {
            if (id != command.Id)
                return BadRequest("The route ID and command ID do not match.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var charakterDto = await _mediator.Send(command);
                return Ok(charakterDto);
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
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCharakter(int id)
        {
            try
            {
                await _mediator.Send(new DeleteCharakterCommand(id));
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
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the charakter");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CharakterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCharakterById(int id)
        {
            try
            {
                var query = new GetCharakterByIdQuery(id);
                var charakterDto = await _mediator.Send(query);
                return Ok(charakterDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the charakter");
            }
        }

        [HttpGet("charakterName/{name}")]
        [ProducesResponseType(typeof(CharakterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCharakterByName(string name)
        {
            try
            {
                var query = new GetCharakterByNameQuery(name);
                var charakterDto = await _mediator.Send(query);
                return Ok(charakterDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the charakter by name");
            }
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(CharakterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCharakters()
        {
            try
            {
                var query = new GetAllCharakterQuery();
                var charakterDto = await _mediator.Send(query);
                return Ok(charakterDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the charakters");
            }
        }
    }
}
