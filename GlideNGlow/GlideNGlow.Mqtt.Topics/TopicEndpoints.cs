namespace GlideNGlow.Mqtt.Topics;

public static class TopicEndpoints
{
    public const string TopicRgb = "{{r: {0},  g:{1},  b:{2}}}";
    public const string SigninTopic = "esp32/+/connected";
    public const string TestTopic = "esp32/acknowledge";
    public const string ButtonTopic = "esp32/+/button";
    public const string TopicSetPixel = "esp32strip/led";
    public const string TopicOnConnect = "esp32strip/connected";
    public const string TopicSetStripSize = "esp32strip/config";
}