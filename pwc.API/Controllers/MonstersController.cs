using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pwc.Application.CQRS.Commands.Charakter;
using pwc.Application.CQRS.Commands.Monster;
using pwc.Application.CQRS.Queries.Charakter;
using pwc.Application.CQRS.Queries.Monster;
using pwc.Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace pwc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonstersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MonstersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new monster.
        /// </summary>
        /// <param name="command">The monster creation data.</param>
        /// <returns>The created monster.</returns>
        /// <response code="201">Returns the newly created monster.</response>
        /// <response code="400">If the request is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(MonsterDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMonster([FromBody] CreateMonsterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            try
            {
                var monsterDto = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetMonsterById), new { id = monsterDto.Id }, monsterDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the monster");
            }
        }

        /// <summary>
        /// Assigns drops or loot to monsters.
        /// </summary>
        /// <param name="command">The monster assignment data.</param>
        /// <returns>The assigned monster to loop relation.</returns>
        /// <response code="201">Returns the relation between drops(items) and monsters.</response>
        /// <response code="400">If the request is invalid.</response>
        [HttpPost("{monsterId}/drops")]
        [ProducesResponseType(typeof(MonsterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDropToMonster(int monsterId, [FromBody] AddDropToMonsterCommand command)
        {
            if (monsterId != command.MonsterId)
                return BadRequest("Route monsterId and command MonsterId do not match.");

            try
            {
                var monsterDto = await _mediator.Send(command);
                return Ok(monsterDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing monster.
        /// </summary>
        /// <param name="id">The monsters's ID.</param>
        /// <param name="command">The updated monster data.</param>
        /// <returns>The updated monster.</returns>
        /// <response code="200">Returns the updated monster.</response>
        /// <response code="400">If the request is invalid or IDs do not match.</response>
        /// <response code="404">If the monster is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MonsterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMonster(int id, [FromBody] UpdateMonsterCommand command)
        {
            if (id != command.Id)
                return BadRequest("The route ID and command ID do not match.");

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var monsterDto = await _mediator.Send(command);
                return Ok(monsterDto);
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
        /// Deletes an monster by ID.
        /// </summary>
        /// <param name="id">The monster's ID.</param>
        /// <response code="204">If the monster was deleted.</response>
        /// <response code="404">If the monster is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMonster(int id)
        {
            try
            {
                await _mediator.Send(new DeleteMonsterCommand(id));
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
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the monster");
            }
        }

        /// <summary>
        /// Gets an monster by its unique ID.
        /// </summary>
        /// <param name="id">The monster's ID.</param>
        /// <returns>The monster DTO.</returns>
        /// <response code="200">Returns the monster.</response>
        /// <response code="404">If the monster is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MonsterDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMonsterById(int id)
        {
            try
            {
                var query = new GetMonsterByIdQuery(id);
                var charakterDto = await _mediator.Send(query);
                return Ok(charakterDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the monster");
            }
        }

        /// <summary>
        /// Gets all monsters.
        /// </summary>
        /// <returns>A list of all monster.</returns>
        /// <response code="200">Returns the list of monsters.</response>
        /// <response code="404">If no monsters are found.</response>
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<MonsterDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllMonster()
        {
            try
            {
                var query = new GetAllMonsterQuery();
                var charakterDto = await _mediator.Send(query);
                return Ok(charakterDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the monster");
            }
        }

        /// <summary>
        /// Gets all monsters by name (partial or full match).
        /// </summary>
        /// <param name="name">The monster name to search for.</param>
        /// <returns>A list of matching monsters.</returns>
        /// <response code="200">Returns the list of monsters.</response>
        /// <response code="404">If no monsters are found.</response>
        [HttpGet("MonsterName/{name}")]
        [ProducesResponseType(typeof(IEnumerable<MonsterDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMonsterByName(string name)
        {
            try
            {
                var query = new GetAllMonsterByNameQuery(name);
                var charakterDto = await _mediator.Send(query);
                return Ok(charakterDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the monster");
            }
        }
    }
}
