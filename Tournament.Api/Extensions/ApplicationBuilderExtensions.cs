using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;

namespace Tournament.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task SeedDataAsync(this IApplicationBuilder builder)
    {
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            // Get the service provider and the database context
            var serviceProvider = scope.ServiceProvider;
            var db = serviceProvider.GetRequiredService<TournamentContext>();

            // Ensure the database is created and migrations are applied
            await db.Database.MigrateAsync();

            if (await db.TournamentDetails.AnyAsync())
            {
                return; // Database has been seeded
            }

            try
            {
                // Try to generate a number of Tournaments through SeedData
                var tournaments = SeedData.GenerateTournaments(4);

                // Add the Tournaments to the database and save changes
                db.AddRange(tournaments);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not seed database: {ex.Message}");
            }
        }
    }
}
