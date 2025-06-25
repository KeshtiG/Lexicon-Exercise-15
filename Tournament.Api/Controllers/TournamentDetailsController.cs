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

    // Constructor
    public TournamentDetailsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // GET: api/TournamentDetails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails()
    {
        // Map all tournaments from the database to a list of TournamentDto objects
        var tournaments = _mapper.Map<IEnumerable<TournamentDto>>
            (await _unitOfWork.TournamentRepository.GetAllAsync());

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
            return NotFound("Tournament does not exist");
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
        // Get the Tournament entity with the assigned ID
        var existingTournament = await _unitOfWork.TournamentRepository.GetAsync(id);

        if (existingTournament == null)
        {
            return NotFound("Tournament does not exist");
        }

        // Update existingTournament with values from DTO
        _mapper.Map(dto, existingTournament);

        // Mark the entity as modified in the repository so changes are tracked for saving
        _unitOfWork.TournamentRepository.Update(existingTournament);

        // Save changes to the database
        await _unitOfWork.CompleteAsync();

        return NoContent(); 
    }

    // POST: api/TournamentDetails
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TournamentDetails>> PostTournamentDetails(TournamentDto dto)
    {
        // Convert DTO to a TournamentDetails entity
        var tournament = _mapper.Map<TournamentDetails>(dto);

        // Add the entity to the list of Tournaments and save changes
        _unitOfWork.TournamentRepository.Add(tournament);
        await _unitOfWork.CompleteAsync();

        // Map the saved entity back to a DTO
        var createdTournament = _mapper.Map<TournamentDto>(tournament);

        // Return a response with a location header for the new entity
        return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournament.Id }, tournament);
    }

    // DELETE: api/TournamentDetails/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTournamentDetails(int id)
    {
        // Get the Tournament entity with the assigned ID
        var tournamentDetails = await _unitOfWork.TournamentRepository.GetAsync(id);

        if (tournamentDetails == null)
        {
            return NotFound("Tournament not found.");
        }

        // Remove the entity from the database and save changes
        _unitOfWork.TournamentRepository.Remove(tournamentDetails);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    //private bool TournamentDetailsExists(int id)
    //{
    //    return _context.TournamentDetails.Any(e => e.Id == id);
    //}
}
