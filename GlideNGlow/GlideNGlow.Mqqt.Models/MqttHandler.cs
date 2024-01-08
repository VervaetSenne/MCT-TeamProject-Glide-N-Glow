using System.Text;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace GlideNGlow.Mqqt.Models;
public class MqttHandler : IDisposable
{
    private readonly string _brokerAddress;
    private readonly ILogger _logger;
    private readonly IMqttClient _mqttClient;

    public MqttHandler(string ip, ILogger? logger)
    {
        _brokerAddress = ip;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        await _mqttClient.DisconnectAsync();

        _logger.LogInformation("MQTT application message is published.");
        
    }

    public async Task Subscribe(String topic, Action<string,string> callback)
    {
        var mqttFactory = new MqttFactory();
        
        await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());

        _mqttClient.ApplicationMessageReceivedAsync += (e) =>
        {
            callback(e.ApplicationMessage.Topic,Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.Array ?? Array.Empty<byte>()));
            return Task.CompletedTask;
        };

        _logger.LogInformation("MQTT application message is subscribed.");

    }

    public void Dispose()
    {
        _mqttClient.Dispose();
    }
}