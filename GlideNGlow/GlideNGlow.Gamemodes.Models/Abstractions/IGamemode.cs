using GlideNGlow.Rendering.Models.Abstractions;

namespace GlideNGlow.Gamemodes.Models.Abstractions;

public interface IGamemode
{
    void Initialize(CancellationToken cancellationToken);

    void Stop();
    Task UpdateAsync(TimeSpan timeSpan, CancellationToken cancellationToken);
    List<RenderObject> GetRenderObjects();

    /// <summary>
    /// Forces the renderer to update the display, can be used when all objects just got removed.
    /// </summary>
    bool ShouldForceRender();

    Task ButtonPressed(int id, CancellationToken cancellationToken);
}