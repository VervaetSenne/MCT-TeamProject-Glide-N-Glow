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
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly ILogger _logger;
    private readonly IMqttClient _mqttClient;

    public MqttHandler(IOptionsMonitor<AppSettings> appSettings, IMqttClient mqttClient, ILogger<MqttHandler> logger)
    {
        _appSettings = appSettings;
        _mqttClient = mqttClient;
        _logger = logger;
    }

    private async Task TryConnectAsync(CancellationToken cancellationToken)
    {
        if (!_mqttClient.IsConnected)
        {
            var brokerAddress = _appSettings.CurrentValue.Ip;
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerAddress)
                .Build();
            await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);
        }
    }

    public async Task SendMessage(string topic, string payload, CancellationToken cancellationToken)
    {
        await TryConnectAsync(cancellationToken);
        
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
        await TryConnectAsync(cancellationToken);
        
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
        await TryConnectAsync(cancellationToken);
        
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
        await TryConnectAsync(cancellationToken);
        
        await _mqttClient.UnsubscribeAsync(topic, cancellationToken);
    }
}