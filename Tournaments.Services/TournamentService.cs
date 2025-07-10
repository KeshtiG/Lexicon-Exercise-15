using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Services.Contracts;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
using Tournament.Core.Exceptions;
using Tournament.Core.Repositories;

namespace Tournament.Services;

public class TournamentService : ITournamentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // Constructor
    public TournamentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<IEnumerable<TournamentDto>> GetAllAsync(bool includeGames)
    {
        // Fetch all tournaments from database
        var tournaments = await _unitOfWork.TournamentRepository.GetAllAsync(includeGames);

        // Check if the list is empty
        if (!tournaments.Any())
        {
            throw new TournamentsNotFoundException();
        }

        // Return the list mapped to DTO:s
        return _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
    }

    public async Task<TournamentDto> GetAsync(int id)
    {
        // Fetch entity
        TournamentDetails? tournament = await GetEntityAsync(id);
        
        // Return tournament mapped to DTO
        return _mapper.Map<TournamentDto> (tournament);
    }

    public async Task UpdateTournamentAsync(int id, UpdateTournamentDto dto)
    {
        // Fetch entity
        var tournament = await GetEntityAsync(id);

        // Update the entity with values from DTO
        _mapper.Map(dto, tournament);

        // Mark the entity as modified so changes are tracked for saving
        _unitOfWork.TournamentRepository.Update(tournament);

        // Save changes to database
        await _unitOfWork.CompleteAsync();
    }

    public async Task<TournamentDto> CreateTournamentAsync(CreateTournamentDto dto)
    {
        // Convert DTO to a TournamentDetails entity
        var tournament = _mapper.Map<TournamentDetails>(dto);

        // Add the created tournament to the database and save changes
        _unitOfWork.TournamentRepository.Add(tournament);
        await _unitOfWork.CompleteAsync();

        // Map the saved entity back to a DTO and return it
        return _mapper.Map<TournamentDto>(tournament);
    }

    public async Task DeleteTournament(int id)
    {
        // Get the Tournament entity with the assigned ID
        TournamentDetails? tournament = await GetEntityAsync(id);

        // Remove the entity from the database and save
        _unitOfWork.TournamentRepository.Remove(tournament);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<UpdateTournamentDto> TournamentToPatchAsync(int id)
    {
        // Fetch entity
        TournamentDetails? tournamentToPatch = await GetEntityAsync(id);

        // Map the patched entity back to a DTO
        return _mapper.Map<UpdateTournamentDto>(tournamentToPatch);
    }

    public async Task<TournamentDetails> GetEntityAsync(int id)
    {
        // Fetch entity
        TournamentDetails? tournament = await _unitOfWork.TournamentRepository.GetAsync(id);

        // Check if the entity exists
        if (tournament == null)
        {
            throw new TournamentNotFoundException(id);
        }
        return tournament;
    }

    public async Task EnsureTournamentExists(int id)
    {
        bool tournamentExists = await _unitOfWork.TournamentRepository.AnyAsync(id);

        if (!tournamentExists)
        {
            throw new TournamentNotFoundException(id);
        }
    }
}
