using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Request;

namespace Tournament.Core.Exceptions;

public abstract class NotFoundException : Exception
{
    public string Title { get; set; }

    protected NotFoundException(string message, string title = "Not found") : base(message)
    {
        Title = title;
    }
}


public class TournamentNotFoundException : NotFoundException
{
    public TournamentNotFoundException(int id) : base($"No Tournament with ID '{id}' was found.", "Tournament Not Found")
    {
    }
}

public class TournamentsNotFoundException : NotFoundException
{
    public TournamentsNotFoundException() : base($"No Tournaments were found in the database.", "No Tournaments Found")
    {
    }
}



public class GameNotFoundException : NotFoundException
{
    public GameNotFoundException(int id, int tournamentId) : base($"No Game with ID '{id}' was found in Tournament with ID '{tournamentId}'.", "Game Not Found")
    {
    }
}

public class GameTitleNotFoundException : NotFoundException
{
    public GameTitleNotFoundException(string title, int tournamentId)
        : base($"No Game with title '{title}' was found in Tournament with ID '{tournamentId}'.", "Game Title Not Found")
    {
    }
}

public class GamesNotFoundException : NotFoundException
{
    public GamesNotFoundException(int tournamentId) : base($"No Games found in Tournament with ID '{tournamentId}'.", "No Games Found")
    {
    }
}