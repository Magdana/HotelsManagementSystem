using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Infrastructure.Data;
using HMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HMS.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();


        var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddScoped<IHotelRepository, HotelRepository>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
