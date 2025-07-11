using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Request;

namespace Tournament.Core.Repositories;

public interface ITournamentRepository
{
    Task<PagedList<TournamentDetails>> GetAllAsync(TournamentRequestParams requestParams);
    Task<TournamentDetails?> GetAsync(int id);
    Task<bool> AnyAsync(int id);
    Task<int> CountGamesAsync(int tournamentId);
    void Add(TournamentDetails tournamentDetails);
    void Update(TournamentDetails tournamentDetails);
    void Remove(TournamentDetails tournamentDetails);
}
