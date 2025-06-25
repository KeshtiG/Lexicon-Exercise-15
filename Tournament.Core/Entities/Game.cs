using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Entities;

public class Game
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Game Name is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Game Name is 30 characters.")]
    public required string Title { get; set; }
    public DateTime StartDate { get; set; }
    public int TournamentId { get; set; }
    public TournamentDetails? Tournament { get; set; }
}
