using GameBattleSystem.Interfaces;
using GameBattleSystem.Models;
using GameBattleSystem.Services;

namespace GameBattleSystem.Tests;

public class FileAndBattleTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "gbs_tests_" + Guid.NewGuid());

    public FileAndBattleTests() => Directory.CreateDirectory(_folder);

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    private class FakeLogger : IBattleLogger
    {
        public List<string> Lines { get; } = new();
        public void Log(string message) => Lines.Add(message);
    }

    [Fact]
    public void SaveThenLoad_KeepsCharacterTypesAndStats()
    {
        var manager = new FileManager(Path.Combine(_folder, "chars.json"));
        var original = new List<Character>
        {
            new Knight(1, "K", 100, 10, 5),
            new Archer(2, "A", 90, 20, 3),
            new Mage(3, "M", 80, 15, 2)
        };

        manager.Save(original);
        var loaded = manager.Load();

        Assert.Equal(3, loaded.Count);
        Assert.IsType<Knight>(loaded[0]);
        Assert.IsType<Archer>(loaded[1]);
        Assert.IsType<Mage>(loaded[2]);
        Assert.Equal(20, loaded[1].Attack);
    }

    [Fact]
    public void Load_MissingFile_ReturnsDefaultsWithWarning()
    {
        var manager = new FileManager(Path.Combine(_folder, "missing.json"));

        var loaded = manager.Load();

        Assert.NotEmpty(loaded);
        Assert.NotEmpty(manager.Warnings);
    }

    [Fact]
    public void Load_MalformedJson_DoesNotCrashAndReturnsDefaults()
    {
        string path = Path.Combine(_folder, "bad.json");
        File.WriteAllText(path, "{ this is not valid json ");
        var manager = new FileManager(path);

        var loaded = manager.Load();

        Assert.NotEmpty(loaded);
        Assert.Contains(manager.Warnings, w => w.Contains("damaged"));
    }

    [Fact]
    public void Load_InvalidEntry_IsSkippedWhileValidOnesAreKept()
    {
        string path = Path.Combine(_folder, "mixed.json");
        File.WriteAllText(path, """
            [
              { "characterType": "Knight", "Id": 1, "Name": "Good", "MaxHealth": 100, "Health": 100, "Attack": 10, "Defence": 5 },
              { "characterType": "Mage",   "Id": 2, "Name": "Bad",  "MaxHealth": -5,  "Health": -5,  "Attack": 10, "Defence": 5 }
            ]
            """);
        var manager = new FileManager(path);

        var loaded = manager.Load();

        Assert.Single(loaded);
        Assert.Equal("Good", loaded[0].Name);
        Assert.Single(manager.Warnings);
    }

    [Fact]
    public void Battle_PlayerKillsEnemy_PlayerWinsAndEnemyDoesNotRetaliate()
    {
        var player = new Knight(1, "Hero", 100, 30, 10);
        var enemy = new Archer(1, "Weakling", 10, 5, 0);
        var logger = new FakeLogger();
        var battle = new Battle(player, enemy, logger);

        battle.TakeTurn(PlayerAction.Attack);

        Assert.Equal(BattleState.PlayerWon, battle.State);
        Assert.Equal(100, player.Health);
        Assert.Contains(logger.Lines, l => l.Contains("BATTLE END"));
    }

    [Fact]
    public void Battle_EnemyRespondsAfterPlayerAction_AndTurnsAfterEndAreRejected()
    {
        var player = new Knight(1, "Hero", 100, 5, 0);
        var enemy = new Archer(1, "Brute", 500, 20, 0);
        var battle = new Battle(player, enemy, new FakeLogger());

        battle.TakeTurn(PlayerAction.Attack);
        Assert.Equal(80, player.Health);

        battle.Quit();
        Assert.Equal(BattleState.Quit, battle.State);
        Assert.Throws<InvalidOperationException>(() => battle.TakeTurn(PlayerAction.Attack));
    }
}
