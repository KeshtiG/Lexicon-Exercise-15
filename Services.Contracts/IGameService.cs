using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;

namespace Services.Contracts;

public interface IGameService
{
    Task<Game> GetEntityAsync(int id, int tournamentId);
    Task<IEnumerable<GameDto>> GetAllAsync(int tournamentId);
    Task<IEnumerable<GameDto>> GetByTitleAsync(string title, int tournamentId);
    Task<GameDto> GetAsync(int id, int tournamentId);
    Task UpdateGameAsync(int id, int tournamentId, UpdateGameDto dto);
    Task<GameDto> CreateGameAsync(CreateGameDto dto, int tournamentId);
    Task DeleteGame(int id, int tournamentId);
    Task<UpdateGameDto> GameToPatchAsync(int id, int tournamentId);

}
