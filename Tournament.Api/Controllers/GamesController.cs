using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tournament.Core.Entities;
using AutoMapper;
using Tournament.Core.Repositories;
using Tournament.Core.Dto;
using Microsoft.AspNetCore.JsonPatch;

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
        // Check if the tournament exists in the database, return error message if not
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
        {
            return NotFound($"A tournament with the ID '{tournamentId}' could not be found.");
        }

        // Map all games from the database to a list of GameDto objects
        var games = _mapper.Map<IEnumerable<GameDto>>
            (await _unitOfWork.GameRepository.GetAllAsync(tournamentId));

        if (!games.Any())
        {
            return NotFound($"No games could be found for the tournament with ID '{tournamentId}'.");
        }

        return Ok(games);
    }

    // GET: api/Games/5
    [HttpGet("{title}")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGame(string title, int tournamentId)
    {
        // Check if the tournament exists in the database, return error message if not
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
        {
            return NotFound($"A tournament with the ID '{tournamentId}' could not be found.");
        }

        // Get the Game entity with the assigned title
        var games = _mapper.Map<IEnumerable<GameDto>>(await _unitOfWork.GameRepository.GetByTitleAsync(title, tournamentId));

        if (!games.Any())
        {
            return NotFound($"No game with the title '{title}' could be found in tournament with ID '{tournamentId}'.");
        }

        return Ok(games);
    }

    // PUT: api/Games/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGame(int id, UpdateGameDto dto, int tournamentId)
    {
        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        // Check if the tournament exists in the database, return error message if not
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
        {
            return NotFound($"A tournament with the ID '{tournamentId}' could not be found.");
        }

        // Get the Game entity with the assigned ID
        Game? game = await _unitOfWork.GameRepository.GetAsync(id, tournamentId);

        if (game == null)
        {
            return NotFound($"Game with ID '{id}' not found in tournament with ID '{tournamentId}'.");
        }

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
        // Check if the tournament exists in the database, return error message if not
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
        {
            return NotFound($"A tournament with the ID '{tournamentId}' could not be found.");
        }

        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

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
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGame(int id, int tournamentId)
    {
        // Check if the tournament exists in the database, return error message if not
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
        {
            return NotFound($"A tournament with the ID '{tournamentId}' could not be found.");
        }

        // Get the Game entity with the assigned ID
        var game = await _unitOfWork.GameRepository.GetAsync(id, tournamentId);
        if (game == null)
        {
            return NotFound($"Game with ID '{id}' not found in tournament with ID '{tournamentId}'.");
        }

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

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> PatchGame(int tournamentId, int id, JsonPatchDocument<UpdateGameDto> patchDoc)
    {
        // Check if the patch document is null and return error if so
        if (patchDoc is null) return BadRequest("No patch document");

        // Check if the tournament exists in the database, return error message if not
        var tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(tournamentId);
        if (!tournamentExists)
        {
            return NotFound($"A tournament with the ID '{tournamentId}' could not be found.");
        }

        var gameToPatch = await _unitOfWork.GameRepository.GetAsync(id, tournamentId);
        if (gameToPatch == null)
        {
            return NotFound($"Game with ID '{id}' not found in tournament with ID '{tournamentId}'.");
        }

        // Map the patched entity back to a DTO
        var dto = _mapper.Map<UpdateGameDto>(gameToPatch);

        // Apply the patch document to the DTO
        patchDoc.ApplyTo(dto, ModelState);

        // Try to validate the patched DTO
        TryValidateModel(dto);

        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        // Map updated DTO values back to the entity before saving
        _mapper.Map(dto, gameToPatch);

        try
        {
            // Try to save changes to the database
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }

        return NoContent();
    }
}
