using GameBattleSystem.Models;

namespace GameBattleSystem.Algorithms;

public static class SearchAlgorithms
{
    /// <summary>Linear search: checks each character in turn. Works on unsorted data.</summary>
    public static Character? LinearSearchById(IReadOnlyList<Character> characters, int id)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].Id == id)
                return characters[i];
        }
        return null;
    }

    /// <summary>Linear search returning every character whose name contains the text.</summary>
    public static List<Character> LinearSearchByName(IReadOnlyList<Character> characters, string text)
    {
        var results = new List<Character>();
        if (string.IsNullOrWhiteSpace(text)) return results;

        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].Name.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase))
                results.Add(characters[i]);
        }
        return results;
    }

    /// <summary>Binary search on a list that is ALREADY sorted by ID (ascending). Returns the index or -1.</summary>
    public static int BinarySearchById(IReadOnlyList<Character> sortedById, int id)
    {
        int low = 0;
        int high = sortedById.Count - 1;

        while (low <= high)
        {
            int mid = low + (high - low) / 2;
            int comparison = sortedById[mid].Id.CompareTo(id);

            if (comparison == 0) return mid;
            if (comparison < 0) low = mid + 1;
            else high = mid - 1;
        }
        return -1;
    }
}
