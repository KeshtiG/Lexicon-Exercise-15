using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Tournament.Core.Entities;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace Tournament.Data.Data;

public class SeedData
{
    public static List<TournamentDetails> GenerateTournaments(int numTournaments)
    {
        // Create a new Faker
        var faker = new Faker<TournamentDetails>("sv").Rules((f, t) =>
        {
            // Create an array with first words
            var firstWords = new[] { "Shadow", "Pixel", "Turbo", "Quantum", "Glitch", "Crimson", "Rogue", "Warrior", "Savage", "Frozen" };

            // Create an array with second words
            var lastWords = new[] { "Cup", "Clash", "Open", "Invitational", "Showdown", "League", "Throwdown", "Rumble", "International", "Arena" };

            // Create a title with a random pair of first and second words
            t.Title = $"{f.PickRandom(firstWords)} {f.PickRandom(lastWords)}";

            // Set start date to between today and 1 year from now
            t.StartDate = f.Date.Between(DateTime.Today, DateTime.Today.AddYears(1));

            // Set end date to 3 months after start date
            t.EndDate = t.StartDate.AddMonths(3);

            // Generate 2–6 games with dates between tournament's start and end dates
            t.Games = GenerateGames(f.Random.Int(min: 2, max: 6), t.StartDate, t.EndDate);
        });

        return faker.Generate(numTournaments);
    }

    public static ICollection<Game>? GenerateGames(int numGames, DateTime startDate, DateTime endDate)
    {
        // Create a new Faker
        var faker = new Faker<Game>("sv").Rules((f, g) =>
        {
            // Create an array with first words
            var firstWords = new[] { "Galactic", "Gigantic", "Turbo", "Mystic", "Robo", "Shadow", "Neon", "Dragon", "Cosmic", "Edge", "Gutter", "Terrible", "Fall", "Battle", "Dynasty" };

            // Create an array with second words
            var lastWords = new[] { "Showdown", "Pioneers", "Turtles", "Arena", "Rumble", "Sprint", "Knights", "Questers", "Dash", "Clash", "Lords", "Hex", "Legion", "Crystal", "Dungeon" };

            // Set title to a random name from the array
            g.Title = $"{f.PickRandom(firstWords)} {f.PickRandom(lastWords)}";

            // Set start date between tournament's start and end dates
            g.StartDate = f.Date.Between(startDate, endDate);
        });

        return faker.Generate(numGames);
    }
}
