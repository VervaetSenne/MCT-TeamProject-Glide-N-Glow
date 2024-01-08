using System;
using System.Net.NetworkInformation;
using GlideNGlow.Mqqt.Models;
using Microsoft.Extensions.Logging;

public class ESPHandler : IDisposable{
     
     private readonly MqttHandler _mqttHandler;
     private List<string> _espList = new List<string>();
     private readonly ILogger _logger;

     public ESPHandler(string ip, ILogger logger, MqttHandler mqttHandler)
     {
         _mqttHandler = mqttHandler;
         _logger = logger;
     }
    

     public async Task AddSignin()
     {
         string signinTopic = "esp32/+/connected";
         await _mqttHandler.Subscribe(signinTopic, (topic,message) =>
         {
             string macAddress = topic.Split('/')[1];
             _espList.Add(macAddress);
             _logger.LogInformation($"Esp {macAddress} signed in: {message}");
         });
         
     }


     public void Dispose()
     {
         _mqttHandler.Dispose();
     }
}