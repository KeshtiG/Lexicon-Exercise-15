using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories;

public class GameRepository : IGameRepository
{
    private readonly TournamentContext _context;

    public GameRepository(TournamentContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Game>> GetAllAsync(int tournamentId)
    {
        return await _context.Games.Where(g => g.TournamentId.Equals(tournamentId)).ToListAsync();
    }

    public async Task<Game?> GetAsync(int id, int tournamentId)
    {
        return await _context.Games
            .Where(g => g.Id == id && g.TournamentId == tournamentId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Game>> GetByTitleAsync(string title, int tournamentId)
    {
        var games = await _context.Games
            .Where(g => g.Title == title && g.TournamentId == tournamentId)
            .ToListAsync();

        return games;
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
