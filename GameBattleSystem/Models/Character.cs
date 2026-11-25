using System.Text.Json.Serialization;
using GameBattleSystem.Interfaces;

namespace GameBattleSystem.Models;

/// <summary>
/// Abstract base class for every fighter. Subclasses supply their own special ability
/// (polymorphism); the base class owns the shared state and the rules for damage.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "characterType")]
[JsonDerivedType(typeof(Knight), "Knight")]
[JsonDerivedType(typeof(Archer), "Archer")]
[JsonDerivedType(typeof(Mage), "Mage")]
public abstract class Character : ISpecialAbility
{
    public const int SpecialUsesPerBattle = 3;

    // Setters are private so stats can only change through the methods below (encapsulation).
    // [JsonInclude] lets System.Text.Json still fill them when loading.
    [JsonInclude] public int Id { get; private set; }
    [JsonInclude] public string Name { get; private set; } = string.Empty;
    [JsonInclude] public int MaxHealth { get; private set; }
    [JsonInclude] public int Health { get; private set; }
    [JsonInclude] public int Attack { get; private set; }
    [JsonInclude] public int Defence { get; private set; }

    [JsonIgnore] public bool IsDefending { get; private set; }
    [JsonIgnore] public int SpecialUsesLeft { get; private set; } = SpecialUsesPerBattle;
    [JsonIgnore] public string Type => GetType().Name;
    [JsonIgnore] public bool IsDefeated => Health <= 0;
    [JsonIgnore] public abstract string SpecialAbilityName { get; }

    // Used by the JSON deserialiser.
    protected Character() { }

    protected Character(int id, string name, int health, int attack, int defence)
    {
        Id = id;
        Name = name;
        MaxHealth = health;
        Health = health;
        Attack = attack;
        Defence = defence;

        if (!IsValid(out string error))
            throw new ArgumentException(error);
    }

    /// <summary>Checks the stats are sensible. Used by the constructor and when loading JSON.</summary>
    public bool IsValid(out string error)
    {
        if (Id <= 0) error = "ID must be a positive number.";
        else if (string.IsNullOrWhiteSpace(Name)) error = "Name cannot be empty.";
        else if (MaxHealth <= 0) error = "Health must be greater than zero.";
        else if (Health <= 0 || Health > MaxHealth) error = "Current health must be between 1 and max health.";
        else if (Attack <= 0) error = "Attack must be greater than zero.";
        else if (Defence < 0) error = "Defence cannot be negative.";
        else error = string.Empty;

        return error.Length == 0;
    }

    /// <summary>Damage this character would deal with a normal attack (always at least 1).</summary>
    public int CalculateDamageAgainst(Character target) => Math.Max(1, Attack - target.Defence / 2);

    public int NormalAttack(Character target) => target.TakeDamage(CalculateDamageAgainst(target));

    /// <summary>Applies damage. If the character is defending, the hit is halved and the guard is used up.</summary>
    public int TakeDamage(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Damage cannot be negative.");

        if (IsDefending)
        {
            amount = Math.Max(1, amount / 2);
            IsDefending = false;
        }

        Health = Math.Max(0, Health - amount);
        return amount;
    }

    public void Defend() => IsDefending = true;

    /// <summary>Restores health up to MaxHealth and returns how much was actually restored.</summary>
    public int Heal(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount cannot be negative.");

        int before = Health;
        Health = Math.Min(MaxHealth, Health + amount);
        return Health - before;
    }

    /// <summary>Template method: shared usage-limit rule, subclass-specific effect.</summary>
    public string UseSpecialAbility(Character target)
    {
        if (SpecialUsesLeft <= 0)
            throw new InvalidOperationException($"{Name} has no special ability uses left.");

        SpecialUsesLeft--;
        return ExecuteSpecial(target);
    }

    protected abstract string ExecuteSpecial(Character target);

    /// <summary>A full-health copy for a battle, so the saved roster is never changed by fighting.</summary>
    public Character CreateBattleCopy()
    {
        var copy = (Character)MemberwiseClone();
        copy.Health = copy.MaxHealth;
        copy.IsDefending = false;
        copy.SpecialUsesLeft = SpecialUsesPerBattle;
        return copy;
    }

    public string GetInfo() =>
        $"ID: {Id} | Name: {Name} | Type: {Type}{Environment.NewLine}" +
        $"HP: {Health}/{MaxHealth} | Attack: {Attack} | Defence: {Defence}{Environment.NewLine}" +
        $"Special: {SpecialAbilityName} ({SpecialUsesLeft} uses left)";

    public override string ToString() =>
        $"[{Id}] {Name} ({Type}) HP {Health}/{MaxHealth}, ATK {Attack}, DEF {Defence}";
}
