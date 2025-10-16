namespace Gamification.Domain.Model;

/// <summary>
/// Value object imutável representando uma quantidade de XP.
/// </summary>
public readonly record struct XpAmount(int Value)
{
    public static XpAmount Zero => new(0);
}
