using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Contracts;
using Tournament.Api.Extensions;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Data.Repositories;
using Tournament.Services;

namespace Tournament.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register TournamentContext with dependency injection, configuring it to use SQL Server
        // with the specified connection string
        builder.Services.AddDbContext<TournamentContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("TournamentContext") ?? throw new InvalidOperationException("Connection string 'TournamentContext' not found.")));

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register NewtonsoftJason, return HttpNotAcceptable if media type is not Json
        builder.Services.AddControllers(opt => opt.ReturnHttpNotAcceptable = true)
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            // Add support for XML
            .AddXmlDataContractSerializerFormatters();

        // Register all service layer and repository services for dependency injection
        builder.Services.ConfigureServiceLayerServices();
        builder.Services.ConfigureRepositories();

        // Register AutoMapper
        builder.Services.AddAutoMapper(typeof(TournamentMappings));

        var app = builder.Build();

        app.ConfigureExceptionHandler();

        // Call async method to populate the database with initial seed data
        await app.SeedDataAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
