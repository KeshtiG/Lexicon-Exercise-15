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
using Tournament.Core.Request;

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


    public async Task<(IEnumerable<TournamentDto> tournamentDtos, MetaData metaData)> GetAllAsync(TournamentRequestParams requestParams)
    {
        // Fetch paged list of entities from the repository
        var pagedList = await _unitOfWork.TournamentRepository.GetAllAsync(requestParams);

        // Map the paged list of entities to a list of DTOs
        var tournamentDtos = _mapper.Map<IEnumerable<TournamentDto>>(pagedList.Items);

        // Check if the list is empty
        if (!tournamentDtos.Any())
        {
            throw new TournamentsNotFoundException();
        }

        // Return the list of DTOs along with metadata for pagination
        return (tournamentDtos, pagedList.MetaData);
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

        // Return the tournament if it exists, otherwise throw an exception
        return tournament ?? throw new TournamentNotFoundException(id);
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
