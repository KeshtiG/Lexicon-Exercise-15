using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public TournamentNotFoundException(int id) : base($"No Tournament with ID '{id}' was found.")
    {
        
    }
}

public class TournamentsNotFoundException : NotFoundException
{
    public TournamentsNotFoundException() : base($"No Tournaments were found.")
    {

    }
}



public class GameNotFoundException : NotFoundException
{
    public GameNotFoundException(int id, int tournamentId) : base($"No Game with ID '{id}' was found in Tournament with ID '{tournamentId}'.")
    {

    }
}

public class GameTitleNotFoundException : NotFoundException
{
    public GameTitleNotFoundException(string title, int tournamentId)
        : base($"No Game with title '{title}' was found in Tournament with ID '{tournamentId}'.")
    {
    }
}

public class GamesNotFoundException : NotFoundException
{
    public GamesNotFoundException(int tournamentId) : base($"No Games found in Tournament with ID '{tournamentId}'.")
    {

    }
}