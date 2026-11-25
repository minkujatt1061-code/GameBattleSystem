using GameBattleSystem.Algorithms;
using GameBattleSystem.Comparers;

namespace GameBattleSystem.Models;

public class DuplicateCharacterException : Exception
{
    public DuplicateCharacterException(int id)
        : base($"A character with ID {id} already exists.") { }
}

/// <summary>Owns the player's collection of characters and the search/sort operations on it.</summary>
public class Player
{
    private readonly List<Character> _characters = new();

    public IReadOnlyList<Character> Characters => _characters;

    public Player(IEnumerable<Character> characters)
    {
        foreach (var c in characters)
            Add(c);
    }

    public void Add(Character character)
    {
        ArgumentNullException.ThrowIfNull(character);
        if (_characters.Exists(c => c.Id == character.Id))
            throw new DuplicateCharacterException(character.Id);
        _characters.Add(character);
    }

    public int NextId() => _characters.Count == 0 ? 1 : _characters.Max(c => c.Id) + 1;

    public Character? FindById(int id) => SearchAlgorithms.LinearSearchById(_characters, id);

    /// <summary>Binary search needs sorted data, so a copy is sorted by ID first.</summary>
    public Character? FindByIdBinary(int id)
    {
        var copy = new List<Character>(_characters);
        SortAlgorithms.BubbleSort(copy, new IdComparer());
        int index = SearchAlgorithms.BinarySearchById(copy, id);
        return index >= 0 ? copy[index] : null;
    }

    public List<Character> SearchByName(string text) => SearchAlgorithms.LinearSearchByName(_characters, text);

    public void Sort(IComparer<Character> comparer) => SortAlgorithms.BubbleSort(_characters, comparer);
}
