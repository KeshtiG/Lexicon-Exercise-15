using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Request;

namespace Tournament.Core.Repositories;

public interface IGameRepository
{
    Task<PagedList<Game>> GetAllAsync(int tournamentId, RequestParams requestParams);
    Task<Game?> GetAsync(int id, int tournamentId);
    Task<PagedList<Game>> GetByTitleAsync(int tournamentId, string title, RequestParams requestParams);
    Task<bool> AnyAsync(int id);
    void Add(Game game);
    void Update(Game game);
    void Remove(Game game);
}