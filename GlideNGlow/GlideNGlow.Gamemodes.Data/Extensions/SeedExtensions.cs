using GlideNGlow.Common.Models;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Data;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Models.Enums;
using GlideNGlow.Gamemodes.Modes;
using GlideNGlow.Gamemodes.Modes.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options.Implementations;
using Newtonsoft.Json;

namespace GlideNGlow.Gamemodes.Data.Extensions;

public static class SeedExtensions
{
    private const string StaticGuid = "e833a0f7-bb93-11ee-8000-3d7f2dafb2d";
    
    public static async Task UpdateDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GlideNGlowDbContext>();

        var games = new[]
        {
            new Game
            {
                Name = "Ghost Race",
                Description = "Face your record head on in a 1 on 1 race!",
                Image = Array.Empty<byte>(),
                AssemblyName = typeof(GhostRace).AssemblyQualifiedName ?? throw new Exception(),
                Settings = JsonConvert.SerializeObject(new Setting[]
                {
                    new()
                    {
                        DisplayName = "Time limit",
                        Name = nameof(GhostRaceSetting.TimeLimit),
                        Type = SettingType.Time
                    }
                }),
                ScoreImportance = ScoreImportance.None
            },
            new Game
            {
                Name = "Chaos Collect",
                Description = "Collect as many of one colour as possible in a race against your friends! Now with pure chaos!",
                Image = Array.Empty<byte>(),
                AssemblyName = typeof(ChaoticCollect).AssemblyQualifiedName ?? throw new Exception(),
                Settings = JsonConvert.SerializeObject(new Setting[]
                {
                    new()
                    {
                        DisplayName = "Player Amount",
                        Name = nameof(ChaoticCollectSettings.PlayerAmount),
                        Type = SettingType.Amount
                    },
                    new()
                    {
                        DisplayName = "Time Limit",
                        Name = nameof(ChaoticCollectSettings.TimeLimit),
                        Type = SettingType.Time
                    }
                }),
                ScoreImportance = ScoreImportance.Highest
            },
            new Game
            {
                Name = "Time Trial",
                Description = "Click the start button and record your fastest time.",
                Image = Array.Empty<byte>(),
                AssemblyName = typeof(TimeTrial).AssemblyQualifiedName ?? throw new Exception(),
                Settings = "{}",
                ScoreImportance = ScoreImportance.Lowest
            }
        };
        if (dbContext.Database.IsInMemory())
        {
            var ids = Enumerable
                .Range(0, games.Length)
                .Select(i => Guid.Parse(StaticGuid + i))
                .ToArray();
            for (var i = 0; i < ids.Length; i++)
            {
                games[i].Id = ids[i];
            }
            dbContext.Games.AddRange(games);
            dbContext.Entries.AddRange(new []
            {
                new Entry
                {
                    Name = "Warre",
                    Score = "8282",
                    GameId = ids[0],
                    DateTime = default
                },
                new Entry
                {
                    Name = "Remco",
                    Score = "5044",
                    GameId = ids[0],
                    DateTime = default
                },
                new Entry
                {
                    Name = "Masimo",
                    Score = "5425",
                    GameId = ids[0],
                    DateTime = default
                },
                new Entry
                {
                    Name = "Senne",
                    Score = "8755",
                    GameId = ids[1],
                    DateTime = default
                },
            });

            var appsettings = scope.ServiceProvider.GetRequiredService<IWritableOptions<AppSettings>>();
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
        else if (!await dbContext.Games.AnyAsync())
        {
            dbContext.Games.AddRange(games);
        }
        else
        {
            return;
        }
        
        await dbContext.SaveChangesAsync();
    }
}