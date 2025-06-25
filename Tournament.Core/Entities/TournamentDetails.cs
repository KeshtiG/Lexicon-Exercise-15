using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Entities;

public class TournamentDetails
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tournament Name is a required field.")]
    [MaxLength(30, ErrorMessage = "Maximum length for the Tournament Name is 40 characters.")]
    public required string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Navigation property to related games
    public ICollection<Game>? Games { get; set; }
}
