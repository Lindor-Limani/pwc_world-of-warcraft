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

        [HttpGet("")]
        [ProducesResponseType(typeof(MonsterDto), StatusCodes.Status200OK)]
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

        [HttpGet("MonsterName/{name}")]
        [ProducesResponseType(typeof(MonsterDto), StatusCodes.Status200OK)]
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
