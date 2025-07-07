using Services.Contracts;
using Tournament.Core.Repositories;
using Tournament.Data.Repositories;
using Tournament.Services;

namespace Tournament.Api.Extensions;

// Assists in organizing and grouping service registrations
public static class ServiceExtensions
{
    // Set up the main logic services so that they can be used anywhere
    public static void ConfigureServiceLayerServices(this IServiceCollection services)
    {
        // Connect interfaces to classes
        services.AddScoped<IServiceManager, ServiceManager>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<IGameService, GameService>();

        // Enable Lazy which makes the services available only when needed
        services.AddLazy<ITournamentService>();
        services.AddLazy<IGameService>();
    }

    // Set up repositories for database communication
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        // Register UnitOfWork for organized operations
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Game & Tournament repositories
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IGameRepository, GameRepository>();

        // Enable repositories to be used with Lazy
        services.AddLazy<ITournamentRepository>();
        services.AddLazy<IGameRepository>();
    }
}

// Set up support for Lazy<T> so services can be injected as Lazy objects
public static class ServiceCollectionExtensions
{
    // Adds a Lazy version of a service to the dependency injection container
    public static IServiceCollection AddLazy<TService>(this IServiceCollection services) where TService : class
    {
        // Register Lazy<TService> as a scoped service.
        // The actual TService will only be created when the Lazy.Value is accessed.
        return services.AddScoped(provider =>
            new Lazy<TService>(() => provider.GetRequiredService<TService>()));
    }
}
