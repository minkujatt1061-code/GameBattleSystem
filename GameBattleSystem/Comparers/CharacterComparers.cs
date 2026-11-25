using GameBattleSystem.Models;

namespace GameBattleSystem.Comparers;

public enum SortCriteria { Id = 1, Name = 2, Health = 3, Attack = 4, Defence = 5 }

/// <summary>
/// Strategy pattern: each subclass is one interchangeable way of ordering characters.
/// The sort algorithm only sees IComparer&lt;Character&gt; and does not care which one it gets.
/// </summary>
public abstract class CharacterComparer : IComparer<Character>
{
    private readonly bool _descending;

    protected CharacterComparer(bool descending) => _descending = descending;

    public int Compare(Character? x, Character? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        int result = CompareCore(x, y);
        return _descending ? -result : result;
    }

    protected abstract int CompareCore(Character x, Character y);

    public static CharacterComparer Create(SortCriteria criteria, bool descending) => criteria switch
    {
        SortCriteria.Id => new IdComparer(descending),
        SortCriteria.Name => new NameComparer(descending),
        SortCriteria.Health => new HealthComparer(descending),
        SortCriteria.Attack => new AttackComparer(descending),
        SortCriteria.Defence => new DefenceComparer(descending),
        _ => throw new ArgumentOutOfRangeException(nameof(criteria), "Unknown sort criteria.")
    };
}

public class IdComparer : CharacterComparer
{
    public IdComparer(bool descending = false) : base(descending) { }
    protected override int CompareCore(Character x, Character y) => x.Id.CompareTo(y.Id);
}

public class NameComparer : CharacterComparer
{
    public NameComparer(bool descending = false) : base(descending) { }
    protected override int CompareCore(Character x, Character y) =>
        string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
}

public class HealthComparer : CharacterComparer
{
    public HealthComparer(bool descending = false) : base(descending) { }
    protected override int CompareCore(Character x, Character y) => x.MaxHealth.CompareTo(y.MaxHealth);
}

public class AttackComparer : CharacterComparer
{
    public AttackComparer(bool descending = false) : base(descending) { }
    protected override int CompareCore(Character x, Character y) => x.Attack.CompareTo(y.Attack);
}

public class DefenceComparer : CharacterComparer
{
    public DefenceComparer(bool descending = false) : base(descending) { }
    protected override int CompareCore(Character x, Character y) => x.Defence.CompareTo(y.Defence);
}
