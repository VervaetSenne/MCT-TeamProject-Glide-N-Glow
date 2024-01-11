using System.Text;
using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace GlideNGlow.Mqqt.Models;
public class MqttHandler : IAsyncDisposable
{
    private readonly string _brokerAddress;
    private readonly ILogger _logger;
    private readonly IMqttClient _mqttClient;

    public MqttHandler(IOptionsMonitor<AppSettings> appSettings, ILogger<MqttHandler> logger)
    {
        _brokerAddress = appSettings.CurrentValue.Ip;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        logger?.LogInformation("MqttHandler created");
        _mqttClient = new MqttFactory().CreateMqttClient();
        
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_brokerAddress)
            .Build();
        
        _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).Wait();
    }

    public async Task SendMessage(String topic, String payload)
    {
        var mqttFactory = new MqttFactory();


        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.ASCII.GetBytes(payload))
            .Build();

        await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        _logger.LogInformation("MQTT application message is published.");
        
    }

    public async Task Subscribe(String topic, Action<string,string> callback, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
    {
        // make sure it is with qos 1
        await _mqttClient.SubscribeAsync(topic, qos);
        var route = topic.Split('/');
        
        _mqttClient.ApplicationMessageReceivedAsync += (e) =>
        {
            var incoming = e.ApplicationMessage.Topic.Split('/');
            for (int i = 0; i < route.Length; i++)
            {
                if (route[i] == "+") continue;
                if (route[i] != incoming[i]) return Task.CompletedTask;
            }
            callback(e.ApplicationMessage.Topic,Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.Array ?? Array.Empty<byte>()));
            return Task.CompletedTask;
        };

        _logger.LogInformation("MQTT application message is subscribed.");

    }

    public async ValueTask DisposeAsync()
    {
        await _mqttClient.DisconnectAsync();
        _mqttClient.Dispose();
    }
}