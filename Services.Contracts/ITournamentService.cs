using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Dto;
using Tournament.Core.Entities;
using Tournament.Core.Request;

namespace Services.Contracts;

public interface ITournamentService
{
    Task<TournamentDetails> GetEntityAsync(int id);
    Task<(IEnumerable<TournamentDto> tournamentDtos, MetaData metaData)> GetAllAsync(TournamentRequestParams requestParams);
    Task<TournamentDto> GetAsync(int id);
    Task UpdateTournamentAsync(int id, UpdateTournamentDto dto);
    Task<UpdateTournamentDto> TournamentToPatchAsync(int id);
    Task<TournamentDto> CreateTournamentAsync(CreateTournamentDto dto);
    Task DeleteTournament(int id);
    Task EnsureTournamentExists(int id);

}
