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

    public async Task<PagedList<TournamentDetails>> GetAllAsync(TournamentRequestParams requestParams)
    {
        // Convert the list to an IQueryable to match the expected argument type for PagedList.CreateAsync
        var tournaments = requestParams.IncludeGames
            ? _context.TournamentDetails.Include(t => t.Games).AsQueryable()
            : _context.TournamentDetails.AsQueryable();

        return await PagedList<TournamentDetails>.CreateAsync(tournaments, requestParams.PageNumber, requestParams.PageSize);
    }

    public async Task<int> CountGamesAsync(int tournamentId)
    {
        return await _context.Games.CountAsync(g => g.TournamentId == tournamentId);
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
