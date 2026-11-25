using GameBattleSystem.Models;

namespace GameBattleSystem.Interfaces;

/// <summary>Implemented by anything that owns a special ability.</summary>
public interface ISpecialAbility
{
    string SpecialAbilityName { get; }
    int SpecialUsesLeft { get; }
    string UseSpecialAbility(Character target);
}
