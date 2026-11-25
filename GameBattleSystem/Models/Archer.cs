using System.Text.Json.Serialization;

namespace GameBattleSystem.Models;

public class Archer : Character
{
    public Archer() { }

    public Archer(int id, string name, int health, int attack, int defence)
        : base(id, name, health, attack, defence) { }

    [JsonIgnore] public override string SpecialAbilityName => "Power Shot";

    protected override string ExecuteSpecial(Character target)
    {
        int damage = CalculateDamageAgainst(target) * 2;
        int dealt = target.TakeDamage(damage);
        return $"{Name} fires a Power Shot at {target.Name} for {dealt} damage!";
    }
}
