using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Dto;

public record UpdateGameDto
{
    [Required(ErrorMessage = "Game Name is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Game Name is 30 characters.")]
    public required string Title { get; init; }
    public DateTime StartDate { get; init; }
}
