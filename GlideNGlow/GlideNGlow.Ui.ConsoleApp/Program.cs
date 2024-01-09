﻿// See https://aka.ms/new-console-template for more information

using GlideNGlow.Mqqt.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


Console.WriteLine("Hello, World!");


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton(provider =>
                new MqttHandler("10.10.10.13", provider.GetRequiredService<ILogger<MqttHandler>>()))
            .AddSingleton(provider => new EspHandler("10.10.10.13",
                provider.GetRequiredService<ILogger<MqttHandler>>(), provider.GetRequiredService<MqttHandler>()));
    })
    .Build();

using var scope = host.Services.CreateScope();

var espHandler = scope.ServiceProvider.GetRequiredService<EspHandler>();
await espHandler.AddSubscriptions();


await host.RunAsync();
