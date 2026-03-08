using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Mapping;
using HMS.Application.Services;
using HMS.Infrastructure.Data;
using HMS.Infrastructure.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HMS.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        //dbConection
        var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        //Repositories
        builder.Services.AddScoped<IHotelRepository, HotelRepository>();
        builder.Services.AddScoped<IRoomRepository, RoomRepository>();
        builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
        builder.Services.AddScoped<IGuestRepository, GuestRepository>();

        //Services
        builder.Services.AddScoped<IHotelService, HotelService>();
        builder.Services.AddScoped<IRoomService, RoomService>();
        builder.Services.AddScoped<IReservationService, ReservationService>();

        //Mapster
        var config = new TypeAdapterConfig();
        MappingConfig.RegisterMappings(config);
        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS API v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
