using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Common.Models;
using GlideNGlow.Core.Data;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Models.Enums;
using GlideNGlow.Core.Services.Installers;
using GlideNGlow.Gamemodes.Handlers.Installers;
using GlideNGlow.Gamemodes.Modes;
using GlideNGlow.Services.Installers;
using GlideNGlow.Socket.Installers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options.Implementations;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .InstallCore(builder.Configuration)
    .InstallServices(builder.Configuration)
    .InstallSockets()
    .InstallGamemodeEngine(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .WithOrigins(new []
                {
                    "127.0.0.1",
                    "127.0.0.1:5500",
                    "localhost",
                    builder.Configuration.GetSection($"{nameof(AppSettings)}:{nameof(AppSettings.Ip)}").Get<string>() ?? "10.10.10.13"
                }
                .SelectMany(s => new []
                {
                    $"https://{s}" ,
                    $"http://{s}"
                })
                .ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

#if DEBUG
    var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<GlideNGlowDbContext>();
    if (dbContext.Database.IsInMemory())
    {
        dbContext.Games.AddRange(new []
        {
            new Game
            {
                Id = Guid.Parse("a6613b43-c4b1-47da-8ba8-6987b1c8341a"),
                Name = "Ghost Race",
                Description = "Face your record head on in a 1 on 1 race!",
                Image = Array.Empty<byte>(),
                AssemblyName = typeof(GhostRace).AssemblyQualifiedName ?? throw new Exception(),
                Settings = JsonConvert.SerializeObject(new Setting[]
                {
                    new()
                    {
                        DisplayName = "",
                        Name = "Time",
                        Type = nameof(Single),
                    }
                }),
                ScoreImportance = ScoreImportance.Lowest
            },
            new Game
            {
                Id = Guid.Parse("11299b8c-e859-4eb2-a3ef-7f88685824a3"),
                Name = "Collect",
                Description = "Collect buttons faster than anyone else!",
                Image = Array.Empty<byte>(),
                AssemblyName = typeof(GhostRace).AssemblyQualifiedName ?? throw new Exception(),
                Settings = JsonConvert.SerializeObject(new Setting[]
                {
                    new()
                    {
                        DisplayName = "",
                        Type = nameof(Single),
                        Name = "Time"
                    }
                }),
                ScoreImportance = ScoreImportance.Lowest
            },
            new Game
            {
                Id = default,
                Name = "TimeTrial",
                Description = "Click the start button and record your own lap time.",
                Image = Array.Empty<byte>(),
                AssemblyName = typeof(GhostRace).AssemblyQualifiedName ?? throw new Exception(),
                Settings = JsonConvert.SerializeObject(new Setting[]
                {
                    new()
                    {
                        DisplayName = "",
                        Type = nameof(Single),
                        Name = "Time"
                    }
                }),
                ScoreImportance = ScoreImportance.Lowest
            }
            // Add more for testing
        });
        dbContext.Entries.AddRange(new []
        {
            new Entry
            {
                Name = "Warre",
                Score = "8282",
                GameId = Guid.Parse("a6613b43-c4b1-47da-8ba8-6987b1c8341a"),
                DateTime = default
            },
            new Entry
            {
                Name = "Remco",
                Score = "5044",
                GameId = Guid.Parse("a6613b43-c4b1-47da-8ba8-6987b1c8341a"),
                DateTime = default
            },
            new Entry
            {
                Name = "Masimo",
                Score = "5425",
                GameId = Guid.Parse("a6613b43-c4b1-47da-8ba8-6987b1c8341a"),
                DateTime = default
            },
            new Entry
            {
                Name = "Senne",
                Score = "8755",
                GameId = Guid.Parse("11299b8c-e859-4eb2-a3ef-7f88685824a3"),
                DateTime = default
            },
        });
        await dbContext.SaveChangesAsync();

        var appsettings = scope.ServiceProvider.GetRequiredService < IWritableOptions<AppSettings>>();
        appsettings.Update(s =>
        {
            s.Strips = new List<LightstripData>
            {
                new()
                {
                    Id = 0,
                    Leds = 100,
                    Length = 3,
                    DistanceFromLast = 0
                },
                new()
                {
                    Id = 1,
                    Leds = 100,
                    Length = 3,
                    DistanceFromLast = 0
                },
                new()
                {
                    Id = 2,
                    Leds = 100,
                    Length = 3,
                    DistanceFromLast = 0
                }
            };
            s.Buttons = new List<LightButtonData>
            {
                new()
                {
                    ButtonNumber = 0,
                    MacAddress = "6F:C0:00:FF:00:13",
                    DistanceFromStart = 10
                },
                new()
                {
                    ButtonNumber = 1,
                    MacAddress = "5B:D9:F9:89:6B:60",
                    DistanceFromStart = 20
                },
                new()
                {
                    ButtonNumber = 2,
                    MacAddress = "86:6A:50:B5:8E:F6",
                    DistanceFromStart = 30
                },
            };
        });
    }
#endif
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHubs();

app.Run();