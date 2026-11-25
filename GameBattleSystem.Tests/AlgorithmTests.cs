using GameBattleSystem.Algorithms;
using GameBattleSystem.Comparers;
using GameBattleSystem.Models;

namespace GameBattleSystem.Tests;

public class AlgorithmTests
{
    private static List<Character> Sample() => new()
    {
        new Knight(3, "Cara", 100, 15, 10),
        new Archer(1, "Alan", 90, 25, 5),
        new Mage(2, "Bea", 80, 20, 4)
    };

    [Fact]
    public void BubbleSort_ByAttackDescending_OrdersCorrectly()
    {
        var list = Sample();

        SortAlgorithms.BubbleSort(list, new AttackComparer(descending: true));

        Assert.Equal(new[] { 25, 20, 15 }, list.Select(c => c.Attack).ToArray());
    }

    [Fact]
    public void BinarySearch_FindsIdInSortedList_AndReturnsMinusOneWhenMissing()
    {
        var list = Sample();
        SortAlgorithms.BubbleSort(list, new IdComparer());

        int found = SearchAlgorithms.BinarySearchById(list, 2);
        int missing = SearchAlgorithms.BinarySearchById(list, 99);

        Assert.Equal("Bea", list[found].Name);
        Assert.Equal(-1, missing);
    }

    [Fact]
    public void Player_SearchByName_IsCaseInsensitiveAndPartial()
    {
        var player = new Player(Sample());

        var results = player.SearchByName("al");

        Assert.Single(results);
        Assert.Equal("Alan", results[0].Name);
    }

    [Fact]
    public void Player_Add_RejectsDuplicateId()
    {
        var player = new Player(Sample());

        Assert.Throws<DuplicateCharacterException>(() => player.Add(new Mage(1, "Clash", 50, 10, 1)));
    }
}
