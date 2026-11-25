using System.Text.Json.Serialization;

namespace GameBattleSystem.Models;

public class Knight : Character
{
    public Knight() { }

    public Knight(int id, string name, int health, int attack, int defence)
        : base(id, name, health, attack, defence) { }

    [JsonIgnore] public override string SpecialAbilityName => "Shield Bash";

    protected override string ExecuteSpecial(Character target)
    {
        int damage = (int)(CalculateDamageAgainst(target) * 1.5) + 5;
        int dealt = target.TakeDamage(damage);
        return $"{Name} uses Shield Bash on {target.Name} for {dealt} damage!";
    }
}
