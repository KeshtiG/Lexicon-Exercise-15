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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournaments(bool includeGames)
    {
        try
        {
            // Try to fetch list of entities from database
            var tournaments = await _serviceManager.TournamentService.GetAllAsync(includeGames);
            return Ok(tournaments);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TournamentDto>> GetTournament(int id)
    {
        try
        {
            // Try to fetch entity from database
            TournamentDto dto = await _serviceManager.TournamentService.GetAsync(id);
            return Ok(dto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTournament(int id, UpdateTournamentDto dto)
    {
        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        try
        {
            // Try to update entity in database and save changes
            await _serviceManager.TournamentService.UpdateTournamentAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TournamentDetails>> CreateTournament(CreateTournamentDto dto)
    {
        if (!ModelState.IsValid)
        {
            // Return validation errors if any
            return UnprocessableEntity(ModelState);
        }

        try
        {
            // Try to add the entity to the list of Tournaments and save changes
            var createdTournament = await _serviceManager.TournamentService.CreateTournamentAsync(dto);
            // Return 201 Created with a link to the new resource and the created DTO
            return CreatedAtAction(nameof(GetTournament), new { id = createdTournament.Id }, createdTournament);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTournament(int id)
    {
        try
        {
            // Try to remove the entity from the database and save changes
            await _serviceManager.TournamentService.DeleteTournament(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }

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

        try
        {
            // Try to update the entity in the database
            await _serviceManager.TournamentService.UpdateTournamentAsync(id, dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }

        return NoContent();
    }





    //private readonly IUnitOfWork _unitOfWork;
    //private readonly IMapper _mapper;

    //// Constructor with depencency injections
    //public TournamentDetailsController(IUnitOfWork unitOfWork, IMapper mapper)
    //{
    //    _unitOfWork = unitOfWork;
    //    _mapper = mapper;
    //}

    //// GET: api/TournamentDetails
    //[HttpGet] ✅
    //public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournament(bool includeGames)
    //{
    //    // Fetch all Tournament entities and map them to a list of TournamentDto objects
    //    var tournaments = includeGames
    //        ? _mapper.Map<IEnumerable<TournamentDto>>
    //            (await _unitOfWork.TournamentRepository.GetAllAsync(true))
    //        : _mapper.Map<IEnumerable<TournamentDto>>
    //            (await _unitOfWork.TournamentRepository.GetAllAsync());

    //    if (!tournaments.Any())
    //    {
    //        return NotFound();
    //    }

    //    // Return a 200 OK response with the list of tournaments DTOs
    //    return Ok(tournaments);
    //}

    //// GET: api/TournamentDetails/5
    //[HttpGet("{id:int}")] ✅
    //public async Task<ActionResult<TournamentDto>> GetTournament(int id)
    //{
    //    TournamentDetails? tournament = await _unitOfWork.TournamentRepository.GetAsync(id);

    //    if (tournament == null)
    //    {
    //        // Return status code Not Found if the ID could not be found in the database.
    //        return NotFound($"A tournament with the ID {id} could not be found.");
    //    }

    //    // Convert the Tournament entity to a TournamentDto using AutoMapper
    //    var dto = _mapper.Map<TournamentDto>(tournament);

    //    return dto;
    //}

    //// PUT: api/TournamentDetails/5
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPut("{id:int}")]
    //public async Task<IActionResult> UpdateTournament(int id, UpdateTournamentDto dto)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        // Return validation errors if any
    //        return UnprocessableEntity(ModelState);
    //    }

    //    // Get the Tournament entity with the assigned ID
    //    var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);
    //    if (tournament == null)
    //    {
    //        // Return status code Not Found if the ID could not be found in the database.
    //        return NotFound($"A tournament with the ID {id} could not be found.");
    //    }

    //    // Update existingTournament with values from DTO
    //    _mapper.Map(dto, tournament);

    //    try
    //    {
    //        // Mark the entity as modified in the repository so changes are tracked for saving
    //        _unitOfWork.TournamentRepository.Update(tournament);
    //        // Try to save changes to the database
    //        await _unitOfWork.CompleteAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Unexpected error: {ex.Message}");
    //    }

    //    return NoContent();
    //}

    //// POST: api/TournamentDetails
    //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    //[HttpPost]
    //public async Task<ActionResult<TournamentDetails>> CreateTournament(CreateTournamentDto dto)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        // Return validation errors if any
    //        return UnprocessableEntity(ModelState);
    //    }

    //    // Convert DTO to a TournamentDetails entity
    //    var tournament = _mapper.Map<TournamentDetails>(dto);

    //    try
    //    {
    //        // Try to add the entity to the list of Tournaments and save changes
    //        _unitOfWork.TournamentRepository.Add(tournament);
    //        await _unitOfWork.CompleteAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Unexpected error: {ex.Message}");
    //    }

    //    // Map the saved entity back to a DTO
    //    var createdTournament = _mapper.Map<TournamentDto>(tournament);

    //    // Return 201 Created with a link to the new resource and the created DTO
    //    return CreatedAtAction(nameof(GetTournament), new { id = tournament.Id }, createdTournament);
    //}

    //// DELETE: api/TournamentDetails/5
    //[HttpDelete("{id:int}")]
    //public async Task<IActionResult> DeleteTournament(int id)
    //{
    //    // Get the Tournament entity with the assigned ID
    //    var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);

    //    if (tournament == null)
    //    {
    //        // Return status code Not Found if the ID could not be found in the database.
    //        return NotFound($"A tournament with the ID '{id}' could not be found.");
    //    }

    //    try
    //    {
    //        // Try to remove the entity from the database and save changes
    //        _unitOfWork.TournamentRepository.Remove(tournament);
    //        await _unitOfWork.CompleteAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Unexpected error: {ex.Message}");
    //    }

    //    return NoContent();
    //}

    //[HttpPatch("{id:int}")]
    //public async Task<ActionResult> PatchTournament(int id, JsonPatchDocument<UpdateTournamentDto> patchDoc)
    //{
    //    // Check if the patch document is null and return error if so
    //    if (patchDoc is null) return BadRequest("No patch document");

    //    // Get the Tournament entity with the assigned ID
    //    var tournamentToPatch = await _unitOfWork.TournamentRepository.GetAsync(id);
    //    if (tournamentToPatch == null)
    //    {
    //        // Return status code Not Found if the ID could not be found in the database.
    //        return NotFound($"A tournament with the ID '{id}' could not be found.");
    //    }

    //    // Map the patched entity back to a DTO
    //    var dto = _mapper.Map<UpdateTournamentDto>(tournamentToPatch);

    //    // Apply the patch document to the DTO
    //    patchDoc.ApplyTo(dto, ModelState);

    //    // Try to validate the patched DTO
    //    TryValidateModel(dto);

    //    if (!ModelState.IsValid)
    //    {
    //        // Return validation errors if any
    //        return UnprocessableEntity(ModelState);
    //    }

    //    // Map updated DTO values back to the entity before saving
    //    _mapper.Map(dto, tournamentToPatch);

    //    try
    //    {
    //        // Try to save changes to the database
    //        await _unitOfWork.CompleteAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Unexpected error: {ex.Message}");
    //    }

    //    return NoContent();
    //}
}
