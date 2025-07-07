using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TournamentContext _context;

    // Private fields holding Lazy-wrapped repository instances
    private readonly Lazy<ITournamentRepository> _tournamentRepository;
    private readonly Lazy<IGameRepository> _gameRepository;

    // Public properties that access the actual repository objects when needed
    public ITournamentRepository TournamentRepository => _tournamentRepository.Value;
    public IGameRepository GameRepository => _gameRepository.Value;

    public UnitOfWork(TournamentContext context, Lazy<ITournamentRepository> tournamentRepository, Lazy<IGameRepository> gameRepository)
    {
        _context = context;
        _tournamentRepository = tournamentRepository;
        _gameRepository = gameRepository;

    }

    public Task CompleteAsync()
    {
        return _context.SaveChangesAsync();
    }



    //public UnitOfWork(TournamentContext context)
    //{
    //    _context = context;

    //    // Lazy object creation which only happens when .Value is used
    //    _tournamentRepository = new Lazy<ITournamentRepository>(() => new TournamentRepository(context));
    //    _gameRepository = new Lazy<IGameRepository>(() => new GameRepository(context));
    //}
}
