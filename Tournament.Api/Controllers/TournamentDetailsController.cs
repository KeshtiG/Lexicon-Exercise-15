using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;
using Tournament.Core.Entities;
using Tournament.Data.Repositories;
using Tournament.Core.Repositories;
using AutoMapper;
using Tournament.Core.Dto;

namespace Tournament.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TournamentDetailsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // Constructor with depencency injections
    public TournamentDetailsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: api/TournamentDetails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails()
    {
        // Fetch all Tournament entities and map them to a list of TournamentDto objects
        var tournaments = _mapper.Map<IEnumerable<TournamentDto>>
            (await _unitOfWork.TournamentRepository.GetAllAsync());

        // Return a 200 OK response with the list of tournaments DTOs
        return Ok(tournaments);
    }

    // GET: api/TournamentDetails/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id)
    {
        // Get the Tournament entity with the assigned ID
        TournamentDetails? tournament = await _unitOfWork.TournamentRepository.GetAsync(id);

        if (tournament == null)
        {
            // Return status code Not Found if the ID could not be found in the database.
            return NotFound($"A tournament with the ID {id} could not be found.");
        }

        // Convert the Tournament entity to a TournamentDto using AutoMapper
        var dto = _mapper.Map<TournamentDto>(tournament);

        return dto;
    }

    // PUT: api/TournamentDetails/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTournamentDetails(int id, UpdateTournamentDto dto)
    {
        if (!ModelState.IsValid)
        {
            // If validation errors occured: return a list of error messages
            return BadRequest(ModelState);
        }

        // Get the Tournament entity with the assigned ID
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);

        if (tournament == null)
        {
            // Return status code Not Found if the ID could not be found in the database.
            return NotFound($"A tournament with the ID {id} could not be found.");
        }

        // Update existingTournament with values from DTO
        _mapper.Map(dto, tournament);

        try
        {
            // Mark the entity as modified in the repository so changes are tracked for saving
            _unitOfWork.TournamentRepository.Update(tournament);
            // Try to save changes to the database
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }

        return NoContent(); 
    }

    // POST: api/TournamentDetails
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TournamentDetails>> PostTournamentDetails(TournamentDto dto)
    {
        if (!ModelState.IsValid)
        {
            // If validation errors occured: return a list of error messages
            return BadRequest(ModelState);
        }

        // Convert DTO to a TournamentDetails entity
        var tournament = _mapper.Map<TournamentDetails>(dto);

        try
        {
            // Try to add the entity to the list of Tournaments and save changes
            _unitOfWork.TournamentRepository.Add(tournament);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }

        // Map the saved entity back to a DTO
        var createdTournament = _mapper.Map<TournamentDto>(tournament);

        // Return 201 Created with a link to the new resource and the created DTO
        return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournament.Id }, createdTournament);
    }

    // DELETE: api/TournamentDetails/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTournamentDetails(int id)
    {
        // Get the Tournament entity with the assigned ID
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);

        if (tournament == null)
        {
            // Return status code Not Found if the ID could not be found in the database.
            return NotFound($"A tournament with the ID {id} could not be found.");
        }

        try
        {
            // Try to remove the entity from the database and save changes
            _unitOfWork.TournamentRepository.Remove(tournament);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }

        return NoContent();
    }
}
