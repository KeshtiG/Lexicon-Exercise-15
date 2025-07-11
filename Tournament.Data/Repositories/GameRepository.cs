using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Request;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories;

public class GameRepository : IGameRepository
{
    private readonly TournamentContext _context;

    public GameRepository(TournamentContext context)
    {
        _context = context;
    }

    public async Task<PagedList<Game>> GetAllAsync(int tournamentId, RequestParams requestParams)
    {
        var games = _context.Games
            .Where(g => g.TournamentId == tournamentId)
            .AsQueryable();

        return await PagedList<Game>.CreateAsync(games, requestParams.PageNumber, requestParams.PageSize);
    }

    public async Task<Game?> GetAsync(int id, int tournamentId)
    {
        return await _context.Games
            .Where(g => g.Id == id && g.TournamentId == tournamentId)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedList<Game>> GetByTitleAsync(int tournamentId, string title, RequestParams requestParams)
    {

        var games = _context.Games
            .Where(g => g.Title == title && g.TournamentId == tournamentId)
            .AsQueryable();

        return await PagedList<Game>.CreateAsync(games, requestParams.PageNumber, requestParams.PageSize);
    }

    public async Task<bool> AnyAsync(int id)
    {
        return await _context.Games.AnyAsync(g => g.Id == id);
    }

    public void Add(Game game)
    {
        _context.Games.Add(game);
    }

    public void Update(Game game)
    {
        _context.Games.Update(game);
    }

    public void Remove(Game game)
    {
        _context.Games.Remove(game);
    }
}
