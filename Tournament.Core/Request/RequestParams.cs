using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Request;

public class RequestParams
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; set; } = 20;
}

public class TournamentRequestParams : RequestParams
{
    public bool IncludeGames { get; set; } = false;
}

public class GameRequestParams : RequestParams
{
    //public int TournamentId { get; set; }
}

public class GameTitleRequestParams : RequestParams
{
    //public int TournamentId { get; set; }
    //public string Title { get; set; } = null!;
}