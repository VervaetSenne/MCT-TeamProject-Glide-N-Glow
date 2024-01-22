using GlideNGlow.Common.Models;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Data;
using GlideNGlow.Core.Models;
using GlideNGlow.Gamemodes.Models;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Services.Installers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options.Implementations;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.InstallServices(builder.Configuration);

#region Backend

builder.Services.AddSingleton<MqttHandler>();
builder.Services.AddSingleton<EspHandler>();
builder.Services.AddSingleton<LightRenderer>(isp => LightRenderer.Create(isp.GetRequiredService<ILogger<LightRenderer>>(),
    isp.GetRequiredService<IOptionsMonitor<AppSettings>>(), isp.GetRequiredService<MqttHandler>()));
builder.Services.AddSingleton<IMqttClient>(_ => new MqttFactory().CreateMqttClient());
builder.Services.AddSingleton<Engine>();

#endregion

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
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
                Id = default,
                Name = "Follow",
                Description = "Follow the light and beat your meat.",
                Image = Array.Empty<byte>(),
                AssemblyName = "",
                Settings = JsonConvert.SerializeObject(new Setting[]
                {
                    new()
                    {
                        Type = nameof(TimeSpan),
                        Name = "Time",
                        Required = true
                    }
                })
            } // Add more for testing
        });
        dbContext.Entries.AddRange(new []
        {
            new Entry
            {
                Name = "Warre",
                Score = "8282"
            },
            new Entry
            {
                Name = "Remco",
                Score = "5044"
            },
            new Entry
            {
                Name = "Masimo",
                Score = "5425"
            },
            new Entry
            {
                Name = "Senne",
                Score = "8755"
            },
        });
        await dbContext.SaveChangesAsync();

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
                },
                new()
                {
                    Id = 0,
                    Leds = 100,
                    Length = 3,
                    DistanceFromLast = 0
                },
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
#endif
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();