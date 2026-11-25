using GameBattleSystem.Comparers;
using GameBattleSystem.Interfaces;
using GameBattleSystem.Models;
using GameBattleSystem.Services;

namespace GameBattleSystem;

/// <summary>Runs the main menu and connects the player, battles, files and logging.</summary>
public class GameSystem
{
    private readonly ICharacterRepository _repository;
    private readonly IBattleLogger _logger;
    private readonly Player _player;
    private readonly Random _random = new();

    public GameSystem(ICharacterRepository repository, IBattleLogger logger)
    {
        _repository = repository;
        _logger = logger;
        _player = new Player(_repository.Load());
    }

    public static GameSystem CreateDefault()
    {
        string dataFolder = Path.Combine(AppContext.BaseDirectory, "Data");
        return new GameSystem(
            new FileManager(Path.Combine(dataFolder, "characters.json")),
            new BattleLogger(Path.Combine(dataFolder, "battle_log.txt")));
    }

    public void Run()
    {
        
        Console.WriteLine("=== Game Character & Battle System ===");
        foreach (string warning in _repository.Warnings)
            Console.WriteLine($"[Warning] {warning}");

        bool running = true;
        while (running)
        {
            try
            {
                ShowMenu();
                int choice = InputHelper.ReadInt("Choose an option: ", 1, 7);

                switch (choice)
                {
                    case 1: StartBattle(); break;
                    case 2: SearchCharacters(); break;
                    case 3: SortCharacters(); break;
                    case 4: ViewCharacterInfo(); break;
                    case 5: AddCharacter(); break;
                    case 6: SaveCharacters(); break;
                    case 7: running = false; break;
                }
            }
            catch (EndOfStreamException)
            {
                running = false;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Console.WriteLine($"File error: {ex.Message}");
            }
            catch (Exception ex) when (ex is ArgumentException or DuplicateCharacterException or InvalidOperationException)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        SaveCharacters();
        Console.WriteLine("Goodbye!");
    }

    private static void ShowMenu()
    {
        Console.WriteLine();
        Console.WriteLine("--- Main Menu ---");
        Console.WriteLine("1. Start battle");
        Console.WriteLine("2. Search characters");
        Console.WriteLine("3. Sort characters");
        Console.WriteLine("4. View character information");
        Console.WriteLine("5. Add a character");
        Console.WriteLine("6. Save characters");
        Console.WriteLine("7. Exit");
    }

    private void ListCharacters()
    {
        if (_player.Characters.Count == 0)
        {
            Console.WriteLine("No characters available.");
            return;
        }

        for (int i = 0; i < _player.Characters.Count; i++)
            Console.WriteLine($"{i + 1}. {_player.Characters[i]}");
    }

    private Character? PickCharacter()
    {
        if (_player.Characters.Count == 0)
        {
            Console.WriteLine("No characters available.");
            return null;
        }

        ListCharacters();
        int pick = InputHelper.ReadInt("Select a character number (0 to cancel): ", 0, _player.Characters.Count);
        return pick == 0 ? null : _player.Characters[pick - 1];
    }

    // ---------- Battle ----------
    private void StartBattle()
    {
        Character? chosen = PickCharacter();
        if (chosen is null) return;

        Character fighter = chosen.CreateBattleCopy();
        Character enemy = CharacterFactory.CreateEnemy(_random);
        var battle = new Battle(fighter, enemy, _logger);

        Console.WriteLine();
        Console.WriteLine($"{fighter.Name} faces {enemy.Name} the {enemy.Type}!");

        while (battle.State == BattleState.InProgress)
        {
            Console.WriteLine();
            Console.WriteLine($"--- Round {battle.Round} ---");
            Console.WriteLine($"{fighter.Name}: {fighter.Health}/{fighter.MaxHealth} HP" +
                              (fighter.IsDefending ? " (defending)" : ""));
            Console.WriteLine($"{enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP");
            Console.WriteLine("1. Attack");
            Console.WriteLine("2. Defend");
            Console.WriteLine($"3. Special ability: {fighter.SpecialAbilityName} ({fighter.SpecialUsesLeft} left)");
            Console.WriteLine("4. View character information");
            Console.WriteLine("5. Quit battle");

            int action = InputHelper.ReadInt("Choose an action: ", 1, 5);

            switch (action)
            {
                case 1: PrintAll(battle.TakeTurn(PlayerAction.Attack)); break;
                case 2: PrintAll(battle.TakeTurn(PlayerAction.Defend)); break;
                case 3: PrintAll(battle.TakeTurn(PlayerAction.Special)); break;
                case 4:
                    Console.WriteLine(fighter.GetInfo());
                    Console.WriteLine(enemy.GetInfo());
                    break;
                case 5: battle.Quit(); break;
            }
        }

        Console.WriteLine();
        Console.WriteLine(battle.State switch
        {
            BattleState.PlayerWon => "Victory! You defeated the enemy.",
            BattleState.EnemyWon => "Defeat... the enemy won.",
            _ => "You left the battle."
        });
    }

    private static void PrintAll(IEnumerable<string> lines)
    {
        foreach (string line in lines)
            Console.WriteLine(line);
    }

    // ---------- Search ----------
    private void SearchCharacters()
    {
        Console.WriteLine("1. Search by ID (linear search)");
        Console.WriteLine("2. Search by ID (binary search)");
        Console.WriteLine("3. Search by name");
        int mode = InputHelper.ReadInt("Choose search type: ", 1, 3);

        if (mode == 3)
        {
            string text = InputHelper.ReadText("Enter name (or part of a name): ", 30);
            var matches = _player.SearchByName(text);
            if (matches.Count == 0) Console.WriteLine("No characters found.");
            foreach (var c in matches) Console.WriteLine(c);
            return;
        }

        int id = InputHelper.ReadInt("Enter ID: ", 1, int.MaxValue);
        Character? found = mode == 1 ? _player.FindById(id) : _player.FindByIdBinary(id);
        Console.WriteLine(found is null ? $"No character with ID {id}." : found.GetInfo());
    }

    // ---------- Sort ----------
    private void SortCharacters()
    {
        Console.WriteLine("Sort by: 1. ID  2. Name  3. Health  4. Attack  5. Defence");
        var criteria = (SortCriteria)InputHelper.ReadInt("Choose criteria: ", 1, 5);
        int order = InputHelper.ReadInt("1. Ascending  2. Descending: ", 1, 2);

        _player.Sort(CharacterComparer.Create(criteria, order == 2));
        Console.WriteLine($"Sorted by {criteria} ({(order == 2 ? "descending" : "ascending")}):");
        ListCharacters();
    }

    // ---------- View / Add / Save ----------
    private void ViewCharacterInfo()
    {
        Character? c = PickCharacter();
        if (c is not null) Console.WriteLine(c.GetInfo());
    }

    private void AddCharacter()
    {
        Console.WriteLine("Types: 1. Knight  2. Archer  3. Mage");
        string type = CharacterFactory.AvailableTypes[InputHelper.ReadInt("Choose type: ", 1, 3) - 1];
        string name = InputHelper.ReadText("Name: ");
        int health = InputHelper.ReadInt("Health (20-500): ", 20, 500);
        int attack = InputHelper.ReadInt("Attack (1-100): ", 1, 100);
        int defence = InputHelper.ReadInt("Defence (0-100): ", 0, 100);

        var character = CharacterFactory.Create(type, _player.NextId(), name, health, attack, defence);
        _player.Add(character);
        Console.WriteLine($"Added: {character}");
    }

    private void SaveCharacters()
    {
        try
        {
            _repository.Save(_player.Characters);
            Console.WriteLine("Characters saved.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.WriteLine($"Could not save characters: {ex.Message}");
        }
    }
}
