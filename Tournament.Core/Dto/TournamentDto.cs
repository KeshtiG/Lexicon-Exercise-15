using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Dto;

public record TournamentDto
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate
    {
        get => StartDate.AddMonths(3);
        init { }
    }

    // Collection of associated GameDto objects
    public IEnumerable<GameDto>? Games { get; init; }
}
