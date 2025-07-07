using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Services.Contracts;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournament.Services;

public class GameService : IGameService
{
    private IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // Constructor
    public GameService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GameDto>> GetAllAsync(int tournamentId)
    {
        // Fetch list of games by Tournament ID
        var games = await _unitOfWork.GameRepository.GetAllAsync(tournamentId);

        // Check if the list is empty
        if (!games.Any())
        {
            throw new KeyNotFoundException($"No games found in tournament with ID '{tournamentId}'.");
        }

        // Return list as DTO:s
        return _mapper.Map<IEnumerable<GameDto>>(games);
    }

    public async Task<GameDto> GetAsync(int id, int tournamentId)
    {
        // Fetch entity
        Game? game = await GetEntityAsync(id, tournamentId);

        // Return enitity mapped to DTO
        return _mapper.Map<GameDto>(game);
    }

    public async Task<IEnumerable<GameDto>> GetByTitleAsync(string title, int tournamentId)
    {
        // Fetch list of games by Title & Tournament ID
        var games = await _unitOfWork.GameRepository.GetByTitleAsync(title, tournamentId);

        // Check if the list is empty
        if (!games.Any())
        {
            throw new KeyNotFoundException($"No games with title '{title}' found in tournament with ID '{tournamentId}'.");
        }

        // Return list as DTO:s
        return _mapper.Map<IEnumerable<GameDto>>(games);
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

        // Check if the entity exists
        if (game == null)
        {
            throw new KeyNotFoundException($"Game with ID '{id}' not found in tournament with ID '{tournamentId}'.");
        }
        return game;
    }
}
