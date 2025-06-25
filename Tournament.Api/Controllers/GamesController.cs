using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tournament.Core.Entities;
using AutoMapper;
using Tournament.Core.Repositories;
using Tournament.Core.Dto;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Api.Controllers;

[Route("api/tournaments/{tournamentId}/games")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // Constructor with depencency injections
    public GamesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: api/Games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGames(int tournamentId)
    {
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
            return NotFound($"A tournament with the ID {tournamentId} could not be found.");

        // Map all games from the database to a list of GameDto objects
        var game = _mapper.Map<IEnumerable<GameDto>>
            (await _unitOfWork.GameRepository.GetAllAsync(tournamentId));

        return Ok(game);
    }

    // GET: api/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetGame(int id, int tournamentId)
    {
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
            return NotFound($"A tournament with the ID {tournamentId} could not be found.");

        // Get the Game entity with the assigned ID
        Game? game = await _unitOfWork.GameRepository.GetAsync(id, tournamentId);

        if (game == null)
        {
            return NotFound($"A game with the ID {id} could not be found.");
        }

        // Convert the Game entity to a GameDto using AutoMapper
        var dto = _mapper.Map<GameDto>(game);

        return dto;
    }

    // PUT: api/Games/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(int id, UpdateGameDto dto, int tournamentId)
    {
        if (!ModelState.IsValid)
        {
            // If validation errors occured: return a list of error messages
            return BadRequest(ModelState);
        }

        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
            return NotFound($"A tournament with the ID {tournamentId} could not be found.");

        // Get the Game entity with the assigned ID
        Game? game = await _unitOfWork.GameRepository.GetAsync(id, tournamentId);

        if (game == null)
            return NotFound($"A game with the ID {id} could not be found.");

        // Update existingGame with values from DTO
        _mapper.Map(dto, game);

        try
        {
            // Mark the entity as modified in the repository so changes are tracked for saving
            _unitOfWork.GameRepository.Update(game);
            // Try to save changes to the database
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }

        return NoContent();
    }

    // POST: api/Games
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame(CreateGameDto dto, int tournamentId)
    {
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
            return NotFound($"A tournament with the ID {tournamentId} could not be found.");

        if (!ModelState.IsValid)
            // If validation errors occured: return a list of error messages
            return BadRequest(ModelState);

        // Convert DTO to a Game entity
        var game = _mapper.Map<Game>(dto);
        game.TournamentId = tournamentId;

        try
        {
            // Try to add the entity to the list of Games and save changes
            _unitOfWork.GameRepository.Add(game);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }

        // Map the saved entity back to a DTO
        var createdGame = _mapper.Map<GameDto>(game);

        // Return 201 Created with a link to the new resource and the created DTO
        return CreatedAtAction(nameof(GetGame), new { id = game.Id, tournamentId = game.TournamentId }, createdGame);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id, int tournamentId)
    {
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
            return NotFound($"A tournament with the ID {tournamentId} could not be found.");

        // Get the Game entity with the assigned ID
        var game = await _unitOfWork.GameRepository.GetAsync(id, tournamentId);

        if (game == null)
            return NotFound($"A game with the ID {id} could not be found.");

        try
        {
            // Try to remove the entity from the database and save changes
            _unitOfWork.GameRepository.Remove(game);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }

        return NoContent();
    }
}
