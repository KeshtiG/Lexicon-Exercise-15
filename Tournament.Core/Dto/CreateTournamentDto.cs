using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Dto;

public record CreateTournamentDto
{
    [Required(ErrorMessage = "Tournament Name is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Tournament Name is 40 characters.")]
    public required string Title { get; init; }
    public DateTime StartDate { get; init; }
}
