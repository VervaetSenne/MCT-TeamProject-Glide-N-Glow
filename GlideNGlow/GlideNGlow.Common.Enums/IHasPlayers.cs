namespace GlideNGlow.Common.Enums;

public interface IHasPlayers<out T>
{
    T PlayerAmount { get; }
}