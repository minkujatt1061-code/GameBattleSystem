using GameBattleSystem.Models;

namespace GameBattleSystem.Tests;

public class CharacterTests
{
    [Fact]
    public void KnightShieldBash_DealsMoreDamageThanNormalAttack()
    {
        var knight = new Knight(1, "Test Knight", 100, 20, 10);
        var normalTarget = new Archer(2, "Target A", 200, 10, 10);
        var specialTarget = new Archer(3, "Target B", 200, 10, 10);

        int normal = knight.NormalAttack(normalTarget);
        knight.UseSpecialAbility(specialTarget);
        int special = 200 - specialTarget.Health;

        Assert.True(special > normal, $"Shield Bash ({special}) should beat a normal attack ({normal}).");
    }

    [Fact]
    public void MageHeal_RestoresHealthButNeverAboveMax()
    {
        var mage = new Mage(1, "Test Mage", 100, 20, 5);
        var dummy = new Knight(2, "Dummy", 100, 10, 0);

        mage.TakeDamage(10);
        mage.UseSpecialAbility(dummy);

        Assert.Equal(100, mage.Health);
    }

    [Fact]
    public void Defending_HalvesTheNextHitOnly()
    {
        var archer = new Archer(1, "Attacker", 100, 30, 0);
        var knight = new Knight(2, "Defender", 100, 10, 0);

        knight.Defend();
        int firstHit = archer.NormalAttack(knight);
        int secondHit = archer.NormalAttack(knight);

        Assert.Equal(15, firstHit);
        Assert.Equal(30, secondHit);
    }

    [Fact]
    public void SpecialAbility_ThrowsWhenNoUsesLeft()
    {
        var archer = new Archer(1, "Archer", 100, 20, 5);
        var target = new Knight(2, "Target", 1000, 10, 0);

        for (int i = 0; i < Character.SpecialUsesPerBattle; i++)
            archer.UseSpecialAbility(target);

        Assert.Throws<InvalidOperationException>(() => archer.UseSpecialAbility(target));
    }

    [Fact]
    public void Constructor_RejectsInvalidStats()
    {
        Assert.Throws<ArgumentException>(() => new Knight(1, "", 100, 10, 5));
        Assert.Throws<ArgumentException>(() => new Mage(1, "Bad", 0, 10, 5));
    }
}
