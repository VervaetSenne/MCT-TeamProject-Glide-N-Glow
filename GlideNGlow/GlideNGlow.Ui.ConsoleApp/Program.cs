using System.Drawing;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Services.Installers;
// using Iot.Device.Nmea0183.Sentences;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options.Implementations;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<MqttHandler>()
            .AddSingleton<EspHandler>()
            .AddSingleton<LightRenderer>()
            //.AddSingleton<LightStrip>()
            .InstallServices(context.Configuration);
    }).Build();
    
using var scope = builder.Services.CreateScope();

var appsettings = scope.ServiceProvider.GetRequiredService<IWritableOptions<AppSettings>>();
appsettings.Update(settings => settings.Ip = "10.10.10.252");
appsettings = scope.ServiceProvider.GetRequiredService<IWritableOptions<AppSettings>>();
var test = appsettings.CurrentValue.Ip;

var renderer = scope.ServiceProvider.GetRequiredService<LightRenderer>();
//LerpRenderObject lerpRenderObject = new LerpRenderObject(0,100, Color.Green, Color.Blue);
//LerpRenderObject lerpRenderObject2 = new LerpRenderObject(100,200, Color.Blue, Color.Red);
//LerpRenderObject lerpRenderObject3 = new LerpRenderObject(200,300, Color.Red, Color.Green);
//renderer.Render(lerpRenderObject);
//renderer.Render(lerpRenderObject2);
//renderer.Render(lerpRenderObject3);

//await renderer.Show();

//await renderer.Update();

renderer.SetStripSize(300);
//LineRenderObject renderObject = new LineRenderObject(-2, 4,Color.Red);
List<IRenderObject> renderObjects = new List<IRenderObject>();
//renderObjects.Add(new LerpRenderObject(0, 20, Color.Red, Color.Blue));
renderObjects.Add(new LerpRenderObject(0, 100, Color.Blue, Color.Green));
renderObjects.Add(new LerpRenderObject(200, 100, Color.Green, Color.Red));
renderObjects.Add(new LerpRenderObject(100, 100, Color.Red, Color.Blue));
// renderObjects.Add(new LineRenderObject(0, 5, Color.Red));
// renderObjects.Add(new LineRenderObject(-10, -5, Color.Blue));
//LerpRenderObject renderObject = new LerpRenderObject(0, 20, Color.Red, Color.Blue);
//renderer.Render(renderObject);
await renderer.Show();
// for(int i = 0; i < 900; i++)
while(true)
{
    renderer.Clear();
    foreach(var obj in renderObjects)
    {
        obj.Move(3);
        renderer.Render(obj);
    }
    //renderObject.Move(3);
    //renderer.Render(renderObject);
    await renderer.Show();
    await Task.Delay(300);
}

/*var espHandler = scope.ServiceProvider.GetRequiredService<EspHandler>();
await espHandler.AddSubscriptions();
var spiDeviceHandler = scope.ServiceProvider.GetRequiredService<SpiDeviceHandler>();
_= spiDeviceHandler.TestAsync();*/

//await builder.RunAsync();