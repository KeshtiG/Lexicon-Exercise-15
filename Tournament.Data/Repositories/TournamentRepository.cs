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

public class TournamentRepository : ITournamentRepository
{
    private readonly TournamentContext _context;

    public TournamentRepository(TournamentContext context)
    {
        _context = context;
    }

    public void Add(TournamentDetails tournament)
    {
        _context.Add(tournament);
    }

    public async Task<TournamentDetails?> GetAsync(int id)
    {
        TournamentDetails? tournament = await _context.TournamentDetails.FindAsync(id);

        return tournament;
    }

    public async Task<IEnumerable<TournamentDetails>> GetAllAsync(bool includeGames = false)
    {
        return includeGames ? await _context.TournamentDetails.Include(t => t.Games).ToListAsync()
                            : await _context.TournamentDetails.ToListAsync();
    }

    public async Task<bool> AnyAsync(int id)
    {
        return await _context.TournamentDetails.AnyAsync(t => t.Id == id);
    }

    public void Update(TournamentDetails tournament)
    {
        _context.Update(tournament);
    }

    public void Remove(TournamentDetails tournament)
    {
        _context.Remove(tournament);
    }
}
