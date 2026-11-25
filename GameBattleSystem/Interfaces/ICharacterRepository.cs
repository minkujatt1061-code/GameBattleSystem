using GameBattleSystem.Models;

namespace GameBattleSystem.Interfaces;

public interface ICharacterRepository
{
    /// <summary>Problems found during the last Load (missing file, bad entries...).</summary>
    IReadOnlyList<string> Warnings { get; }

    List<Character> Load();
    void Save(IEnumerable<Character> characters);
}
