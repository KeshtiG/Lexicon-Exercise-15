using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Services.Contracts;
using Tournament.Core.Repositories;

namespace Tournament.Services;

// ServiceManager delays service creation until first use
public class ServiceManager : IServiceManager
{
    // Lazy-wrapped service instances, created on first use
    private readonly Lazy<ITournamentService> _tournamentService;
    private readonly Lazy<IGameService> _gameService;

    // Access the actual service instances when needed
    public ITournamentService TournamentService => _tournamentService.Value;
    public IGameService GameService => _gameService.Value;

    // Constructor that Lazy-loads the injected services
    public ServiceManager(Lazy<ITournamentService> tournamentService, Lazy<IGameService> gameService)
    {
        _tournamentService = tournamentService;
        _gameService = gameService;
    }
}
