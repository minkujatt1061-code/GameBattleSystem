using System.Text.Json.Serialization;

namespace GameBattleSystem.Models;

public class Mage : Character
{
    private const int HealAmount = 30;

    public Mage() { }

    public Mage(int id, string name, int health, int attack, int defence)
        : base(id, name, health, attack, defence) { }

    [JsonIgnore] public override string SpecialAbilityName => "Heal";

    protected override string ExecuteSpecial(Character target)
    {
        // The Mage heals itself; the target is the opponent and is unaffected.
        int restored = Heal(HealAmount);
        return $"{Name} casts Heal and restores {restored} health!";
    }
}
