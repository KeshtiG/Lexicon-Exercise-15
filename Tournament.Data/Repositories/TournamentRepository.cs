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

    public void Add(TournamentDetails tournamentDetails)
    {
        _context.Add(tournamentDetails);
    }

    public async Task<TournamentDetails> GetAsync(int id)
    {
        var tournamentDetails = await _context.TournamentDetails.FindAsync(id);
        if (tournamentDetails == null)
        {
            throw new InvalidOperationException($"Tournament with ID {id} not found.");
        }
        return tournamentDetails;
    }

    public async Task<IEnumerable<TournamentDetails>> GetAllAsync()
    {
        return await _context.TournamentDetails.Include(t => t.Games).ToListAsync();
    }

    public async Task<bool> AnyAsync(int id)
    {
        return await _context.TournamentDetails.AnyAsync(t => t.Id == id);
    }

    public void Update(TournamentDetails tournamentDetails)
    {
        _context.Update(tournamentDetails);
    }

    public void Remove(TournamentDetails tournamentDetails)
    {
        _context.Remove(tournamentDetails);
    }
}
