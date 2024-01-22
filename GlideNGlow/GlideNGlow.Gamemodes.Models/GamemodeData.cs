using GlideNGlow.Core.Models;
using GlideNGlow.Gamemodes.Models.Abstractions;

namespace GlideNGlow.Gamemodes.Models;

public class GamemodeData
{
    public required Game Game { get; set; }
    public required IGamemode Gamemode { get; set; }
}