using GlideNGlow.Common.Enums;
using GlideNGlow.Core.Models;
using GlideNGlow.Gamemodes.Modes;

namespace GlideNGlow.Gamemodes.Constants;

public static class CalibrateMode
{
    public static readonly Game Instance = new Game
    {
        Name = "Callibrate",
        Description = "Callibration",
        Image = Array.Empty<byte>(),
        AssemblyName = typeof(CalibrationMode).AssemblyQualifiedName ?? throw new Exception(),
        Settings = "",
        ScoreImportance = ScoreImportance.None,
        ContentType = ContentType.None
    };
}