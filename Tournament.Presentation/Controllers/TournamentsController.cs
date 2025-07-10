using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tournament.Core.Dto;
using Services.Contracts;
using Tournament.Core.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Tournament.Core.Exceptions;

namespace Tournament.Presentation.Controllers;

[Route("api/tournaments")]
[ApiController]
public class TournamentsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public TournamentsController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    // GET ALL TOURNAMENTS
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournaments(bool includeGames)
    {
        // Fetch list of entities from database
        var tournaments = await _serviceManager.TournamentService.GetAllAsync(includeGames);
        return Ok(tournaments);
    }

    // GET TOURNAMENT BY ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TournamentDto>> GetTournament(int id)
    {
        // Fetch the entity with the assigned ID
        TournamentDto dto = await _serviceManager.TournamentService.GetAsync(id);
        return Ok(dto);
    }

    // CREATE TOURNAMENT
    [HttpPost]
    public async Task<ActionResult<TournamentDetails>> CreateTournament(CreateTournamentDto dto)
    {
        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        // Add the entity to the list of Tournaments and save changes
        var createdTournament = await _serviceManager.TournamentService.CreateTournamentAsync(dto);

        // Return 201 Created with a link to the new resource and the created DTO
        return CreatedAtAction(nameof(GetTournament), new { id = createdTournament.Id }, createdTournament);
    }

    // UPDATE TOURNAMENT
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTournament(int id, UpdateTournamentDto dto)
    {
        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        // Update the entity with values from DTO
        await _serviceManager.TournamentService.UpdateTournamentAsync(id, dto);
        return NoContent();
    }

    // PATCH TOURNAMENT
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> PatchTournament(int id, JsonPatchDocument<UpdateTournamentDto> patchDoc)
    {
        // Check if the patch document is null and return error if so
        if (patchDoc is null) return BadRequest("No valid patch document found.");

        // Fetch the tournament to patch (mapped to a DTO)
        var dto = await _serviceManager.TournamentService.TournamentToPatchAsync(id);

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
        await _serviceManager.TournamentService.UpdateTournamentAsync(id, dto);

        return NoContent();
    }

    // DELETE TOURNAMENT
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTournament(int id)
    {
        // Delete the entity from the database and save changes
        await _serviceManager.TournamentService.DeleteTournament(id);
        return NoContent();
    }
}
