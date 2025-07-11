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

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // Constructor
    public GameService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<GameDto> gameDtos, MetaData metaData)> GetAllAsync(int tournamentId, RequestParams requestParams)
    {
        // Fetch paged list of entities
        var pagedList = await _unitOfWork.GameRepository.GetAllAsync(tournamentId, requestParams);

        // Map the paged list of entities to a list of DTOs
        var gameDtos = _mapper.Map<IEnumerable<GameDto>>(pagedList.Items);

        // Check if the list is empty
        if (!gameDtos.Any())
        {
            throw new GamesNotFoundException(tournamentId);
        }

        // Return the list of DTOs along with metadata for pagination
        return (gameDtos, pagedList.MetaData);
    }

    public async Task<GameDto> GetByIdAsync(int id, int tournamentId)
    {
        // Fetch entity
        Game? game = await GetEntityAsync(id, tournamentId);

        // Return enitity mapped to DTO
        return _mapper.Map<GameDto>(game);
    }

    public async Task<(IEnumerable<GameDto> gameDtos, MetaData metaData)> GetByTitleAsync(string title, int tournamentId, RequestParams requestParams)
    {
        // Fetch paged list of entities by title and ID
        var pagedList = await _unitOfWork.GameRepository.GetByTitleAsync(tournamentId, title, requestParams);

        // Map the paged list of entities to a list of DTOs
        var gameDtos = _mapper.Map<IEnumerable<GameDto>>(pagedList.Items);

        // Check if the list is empty
        if (!gameDtos.Any())
        {
            throw new GameTitleNotFoundException(title, tournamentId);
        }

        // Return the list of DTOs along with metadata for pagination
        return (gameDtos, pagedList.MetaData);
    }

    public async Task UpdateGameAsync(int id, int tournamentId, UpdateGameDto dto)
    {
        // Fetch entity
        var game = await GetEntityAsync(id, tournamentId);

        // Update the entity with values from DTO
        _mapper.Map(dto, game);

        // Mark the entity as modified so changes are tracked for saving
        _unitOfWork.GameRepository.Update(game);

        // Save changes to database
        await _unitOfWork.CompleteAsync();
    }

    public async Task<GameDto> CreateGameAsync(CreateGameDto dto, int tournamentId)
    {
        // Define the maximum number of games allowed
        int maxGameCount = 10;

        // Get the current count of games in the tournament
        var gameCount = await _unitOfWork.TournamentRepository.CountGamesAsync(tournamentId);

        // Check if the maximum game count has been reached
        if (gameCount >= maxGameCount)
        {
            throw new GameLimitReachedException(maxGameCount);
        }

        // Convert DTO to a Game entity and set the ID
        var game = _mapper.Map<Game>(dto);
        game.TournamentId = tournamentId;

        // Add the created game to the database and save changes
        _unitOfWork.GameRepository.Add(game);
        await _unitOfWork.CompleteAsync();

        // Map the saved entity back to a DTO and return it
        return _mapper.Map<GameDto>(game);
    }

    public async Task DeleteGame(int id, int tournamentId)
    {
        // Fetch entity
        Game? game = await GetEntityAsync(id, tournamentId);

        // Remove the entity from the database and save
        _unitOfWork.GameRepository.Remove(game);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<UpdateGameDto> GameToPatchAsync(int id, int tournamentId)
    {
        // Fetch entity
        Game? gameToPatch = await GetEntityAsync(id, tournamentId);

        // Map the patched entity back to a DTO
        return _mapper.Map<UpdateGameDto>(gameToPatch);
    }

    public async Task<Game> GetEntityAsync(int id, int tournamentId)
    {
        // Fetch entity
        Game? game = await _unitOfWork.GameRepository.GetAsync(id, tournamentId);

        // Return entity or throw exception if not found
        return game ?? throw new GameNotFoundException(id, tournamentId);
    }
}
