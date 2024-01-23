namespace GlideNGlow.Common.Abstractions;

public interface IAsyncLifeCycle
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}