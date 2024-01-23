using GlideNGlow.Rendering.Models.Abstractions;

namespace GlideNGlow.Gamemodes.Models.Abstractions;

public interface IGamemode
{
    void Initialize();

    void Stop();
    Task UpdateAsync(TimeSpan timeSpan);
    List<RenderObject> GetRenderObjects();

    Task ButtonPressed(int id);
}