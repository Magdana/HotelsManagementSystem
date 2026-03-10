using HMS.Application.Interfaces;
using HMS.Application.Interfaces.RepositoryInterfaces;
using HMS.Application.Interfaces.ServiceInterfaces;
using HMS.Application.Mapping;
using HMS.Application.Services;
using HMS.Application.Settings;
using HMS.Infrastructure.Data;
using HMS.Infrastructure.Repositories;
using HMS.Infrastructure.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;

namespace HMS.WebAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Enter the Bearer Authorization string as following: Generated-JWT-Token"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

        });

        //dbConection
        var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // JWT Settings
        var jwtSection = builder.Configuration.GetSection("JwtSettings");
        builder.Services.Configure<JwtSettings>(jwtSection);

        // JWT Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSection["Secret"]!))
                };
            });
        builder.Services.AddAuthorization();



        // Repositories
        builder.Services.AddScoped<IHotelRepository, HotelRepository>();
        builder.Services.AddScoped<IRoomRepository, RoomRepository>();
        builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
        builder.Services.AddScoped<IGuestRepository, GuestRepository>();
        builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

        // Services
        builder.Services.AddScoped<IHotelService, HotelService>();
        builder.Services.AddScoped<IRoomService, RoomService>();
        builder.Services.AddScoped<IManagerService, ManagerService>();
        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddScoped<IGuestService, GuestService>();
        builder.Services.AddScoped<IReservationService, ReservationService>();
        builder.Services.AddScoped<IAuthService, AuthService>();


        //Mapster
        var config = new TypeAdapterConfig();
        MappingConfig.RegisterMappings(config);
        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();

        // Utilities
        builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS API v1");
                options.EnablePersistAuthorization();
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var managerRepo = scope.ServiceProvider.GetRequiredService<IManagerRepository>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var adminExists = await managerRepo.ExistsAsync(m => m.Role == "Admin");
            if (!adminExists)
            {
                await managerRepo.AddAsync(new HMS.Domain.Entities.Manager
                {
                    FirstName = "Seed",
                    LastName = "Admin",
                    PersonalNumber = "0000000000",
                    Email = "admin@hms.com",
                    PhoneNumber = "0000000000",
                    PasswordHash = passwordHasher.Hash("Admin@123"),
                    Role = "Admin"
                });
                await managerRepo.SaveAsync();
            }
        }

        await app.RunAsync();
    }
}
