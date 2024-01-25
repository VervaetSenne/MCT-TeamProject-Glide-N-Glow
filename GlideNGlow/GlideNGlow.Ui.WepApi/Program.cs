using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Services.Installers;
using GlideNGlow.Gamemodes.Data.Extensions;
using GlideNGlow.Gamemodes.Handlers.Installers;
using GlideNGlow.Services.Installers;
using GlideNGlow.Socket.Installers;

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
}

await app.Services.UpdateDatabaseAsync();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();
app.MapHubs();

app.Run();