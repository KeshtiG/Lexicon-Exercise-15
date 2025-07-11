using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Contracts;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
using Tournament.Core.Request;

namespace Tournament.Presentation.Controllers;

[Route("api/tournaments/{tournamentId}/games")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public GamesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    // GET ALL GAMES
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGames(int tournamentId, [FromQuery] GameRequestParams requestParams)
    {
        // Checks if the tournament exists, returns error message if not
        await _serviceManager.TournamentService.EnsureTournamentExists(tournamentId);

        // Fetch list of Games
        var pagedResult = await _serviceManager.GameService.GetAllAsync(tournamentId, requestParams);

        // Append pagination metadata to the response headers
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        // Return the list of Game DTOs
        return Ok(pagedResult.gameDtos);
    }

    // GET GAME BY TITLE
    [HttpGet("by-title")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGameByTitle(
        int tournamentId,
        [FromQuery] RequestParams requestParams,
        [FromQuery, BindRequired] string title)
    {

        // Fetch Game by Title
        var pagedResult = await _serviceManager.GameService.GetByTitleAsync(title, tournamentId, requestParams);

        // Append pagination metadata to the response headers
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        // Return the list of Game DTOs
        return Ok(pagedResult.gameDtos);
    }

    // GET GAME BY ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GameDto>> GetGameById(int id, int tournamentId)
    {
        // Fetch Game by ID
        GameDto dto = await _serviceManager.GameService.GetByIdAsync(id, tournamentId);
        return Ok(dto);
    }

    // CREATE GAME
    [HttpPost]
    public async Task<ActionResult<Game>> CreateGame(CreateGameDto dto, int tournamentId)
    {
        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        // Checks if the tournament exists, returns error message if not
        await _serviceManager.TournamentService.EnsureTournamentExists(tournamentId);

        // Add the entity to the list of Tournaments and save changes
        var createdGame = await _serviceManager.GameService.CreateGameAsync(dto, tournamentId);

        // Return 201 Created with a link to the new resource and the created DTO
        return CreatedAtAction(nameof(GetGameById), new { tournamentId = createdGame.TournamentId, id = createdGame.Id }, createdGame);
    }

    // UPDATE GAME
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGame(int id, UpdateGameDto dto, int tournamentId)
    {
        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        // Checks if the tournament exists, returns error message if not
        await _serviceManager.TournamentService.EnsureTournamentExists(tournamentId);

        // Uupdate entity in the database and save changes
        await _serviceManager.GameService.UpdateGameAsync(id, tournamentId, dto);
        return NoContent();
    }

    // PATCH GAME
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> PatchGame(int id, int tournamentId, JsonPatchDocument<UpdateGameDto> patchDoc)
    {
        // Check if the patch document is null and return error if so
        if (patchDoc is null) return BadRequest("No valid patch document found.");

        // Fetch the tournament to patch (mapped to a DTO)
        var dto = await _serviceManager.GameService.GameToPatchAsync(id, tournamentId);

        // Apply the patch document to the DTO
        patchDoc.ApplyTo(dto, ModelState);

        // Try to validate the patched DTO
        TryValidateModel(dto);

        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        // Update the entity in the database
        await _serviceManager.GameService.UpdateGameAsync(id, tournamentId, dto);

        return NoContent();
    }

    // DELETE GAME
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGame(int id, int tournamentId)
    {
        // Delete the entity from the database and save changes
        await _serviceManager.GameService.DeleteGame(id, tournamentId);
        return NoContent();
    }
}
