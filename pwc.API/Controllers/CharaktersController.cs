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

        /// <summary>
        /// Creates a new character.
        /// </summary>
        /// <param name="command">The character creation data.</param>
        /// <returns>The created character.</returns>
        /// <response code="201">Returns the newly created character.</response>
        /// <response code="400">If the request is invalid.</response>
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

        /// <summary>
        /// Equips an item to a character.
        /// </summary>
        /// <param name="charakterID">The character's ID.</param>
        /// <param name="itemID">The item's ID to equip.</param>
        /// <returns>The updated character with the equipped item.</returns>
        /// <response code="200">Returns the updated character.</response>
        /// <response code="404">If the character or item is not found.</response>
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

        /// <summary>
        /// Updates an existing character.
        /// </summary>
        /// <param name="id">The character's ID.</param>
        /// <param name="command">The updated character data.</param>
        /// <returns>The updated character.</returns>
        /// <response code="200">Returns the updated character.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the character is not found.</response>
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


        /// <summary>
        /// Deletes a character by ID.
        /// </summary>
        /// <param name="id">The character's ID.</param>
        /// <response code="204">If the character was deleted.</response>
        /// <response code="404">If the character is not found.</response>
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

        /// <summary>
        /// Gets a character by its unique ID.
        /// </summary>
        /// <param name="id">The character's ID.</param>
        /// <returns>The character DTO.</returns>
        /// <response code="200">Returns the character.</response>
        /// <response code="404">If the character is not found.</response>
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

        /// <summary>
        /// Gets all characters with the specified name.
        /// </summary>
        /// <param name="name">The character's name.</param>
        /// <returns>A list of matching characters.</returns>
        /// <response code="200">Returns the list of characters.</response>
        /// <response code="404">If no characters are found.</response>
        [HttpGet("charakterName/{name}")]
        [ProducesResponseType(typeof(IEnumerable<CharakterDto>), StatusCodes.Status200OK)]
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

        /// <summary>
        /// Gets all characters.
        /// </summary>
        /// <returns>A list of all characters.</returns>
        /// <response code="200">Returns the list of characters.</response>
        /// <response code="404">If no characters are found.</response>

        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<CharakterDto>), StatusCodes.Status200OK)]
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
