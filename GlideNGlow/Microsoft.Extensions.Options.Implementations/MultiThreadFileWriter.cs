using System.Collections.Concurrent;
using Microsoft.Extensions.Options.Implementations.Models;

namespace Microsoft.Extensions.Options.Implementations;

public class MultiThreadFileWriter
{
    private static readonly BlockingCollection<LogMessage> LogMessages = new();
    private static bool _isRunning;
        
    private readonly CancellationTokenSource _source = new();
    private readonly CancellationToken _token;

    public MultiThreadFileWriter()
    {
        _token = _source.Token;
        if (!_isRunning)
            Task.Run(WriteToFile, _token);
    }
        
    public void WriteLine(LogMessage message)
    {
        LogMessages.Add(message, _token);
    }
        
    private async void WriteToFile()
    {
        _isRunning = true;
        foreach (var log in LogMessages.GetConsumingEnumerable(_token))
        {
            var success = false;
            while (!success)
            {
                try
                {
                    if (!File.Exists(log.Filepath))
                    {
                        await using var sw = File.CreateText(log.Filepath);
                        await sw.WriteLineAsync(log.Text);
                        await sw.FlushAsync();
                    }
                    else
                    {
                        await File.WriteAllTextAsync(log.Filepath, log.Text, _token);
                    }
                    success = true;
                }
                catch
                {
                    await Task.Delay(1, _token);
                }
            }
        }
        _isRunning = false;
    }
}