using GameBattleSystem.Models;

namespace GameBattleSystem.Services;

/// <summary>Factory pattern: the only place that decides which concrete Character class to create.</summary>
public static class CharacterFactory
{
    public static readonly string[] AvailableTypes = { "Knight", "Archer", "Mage" };

    public static Character Create(string type, int id, string name, int health, int attack, int defence)
    {
        return (type ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "knight" => new Knight(id, name, health, attack, defence),
            "archer" => new Archer(id, name, health, attack, defence),
            "mage" => new Mage(id, name, health, attack, defence),
            _ => throw new ArgumentException($"Unknown character type '{type}'. Choose Knight, Archer or Mage.")
        };
    }

    /// <summary>Builds a random computer-controlled enemy (ID 1 is fine: enemies are never stored in the roster).</summary>
    public static Character CreateEnemy(Random random)
    {
        return random.Next(3) switch
        {
            0 => new Knight(1, "Dark Knight", 110, 17, 10),
            1 => new Archer(1, "Rogue Archer", 85, 22, 5),
            _ => new Mage(1, "Evil Mage", 80, 19, 4)
        };
    }
}
