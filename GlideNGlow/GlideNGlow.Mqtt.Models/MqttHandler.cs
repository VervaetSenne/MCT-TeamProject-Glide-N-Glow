using System.Text;
using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace GlideNGlow.Mqqt.Models;
public class MqttHandler
{
    private readonly ILogger _logger;
    private readonly IMqttClient _mqttClient;

    private MqttHandler(IMqttClient mqttClient, ILogger<MqttHandler> logger)
    {
        _mqttClient = mqttClient;
        _logger = logger;
        logger.LogInformation("MqttHandler created");
    }

    public static async Task<MqttHandler> Create(IOptionsMonitor<AppSettings> appSettings, ILogger<MqttHandler> logger, IMqttClient mqttClient, CancellationToken cancellationToken)
    {
        var handler = new MqttHandler(mqttClient, logger);
        
        var brokerAddress = appSettings.CurrentValue.Ip;
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerAddress)
            .Build();
        await handler._mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);
        
        return handler;
    }

    public async Task SendMessage(string topic, string payload, CancellationToken cancellationToken)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.ASCII.GetBytes(payload))
            .Build();

        await _mqttClient.PublishAsync(applicationMessage, cancellationToken);

        _logger.LogInformation("MQTT application message is published.");
    }

    public async Task Subscribe(string topic, Func<string, string, Task> callback, CancellationToken cancellationToken,
        MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
    {
        await _mqttClient.SubscribeAsync(topic, qos, cancellationToken);
        var route = topic.Split('/');
        
        _mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var incoming = e.ApplicationMessage.Topic.Split('/');
            if (route.Where((t, i) => t != "+" && t != incoming[i]).Any())
            {
                return;
            }
            await callback(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.Array ?? Array.Empty<byte>()));
        };

        _logger.LogInformation("MQTT application message is subscribed.");
    }
    
    public async Task Subscribe(string topic, Action<string, string> callback, CancellationToken cancellationToken,
        MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.AtLeastOnce)
    {
        await _mqttClient.SubscribeAsync(topic, qos, cancellationToken);
        var route = topic.Split('/');
        
        _mqttClient.ApplicationMessageReceivedAsync += (e) =>
        {
            var incoming = e.ApplicationMessage.Topic.Split('/');
            if (route.Where((t, i) => t != "+" && t != incoming[i]).Any())
            {
                return Task.CompletedTask;
            }
            callback(e.ApplicationMessage.Topic, Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.Array ?? Array.Empty<byte>()));
            return Task.CompletedTask;
        };

        _logger.LogInformation("MQTT application message is subscribed.");
    }
    
    public async Task Unsubscribe(string topic, CancellationToken cancellationToken)
    {
        await _mqttClient.UnsubscribeAsync(topic, cancellationToken);
    }
}