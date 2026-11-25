using System.Text.Json;
using GameBattleSystem.Interfaces;
using GameBattleSystem.Models;

namespace GameBattleSystem.Services;

/// <summary>Saves and loads the character roster as JSON, recovering from missing or damaged files.</summary>
public class FileManager : ICharacterRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _path;
    private readonly List<string> _warnings = new();

    public IReadOnlyList<string> Warnings => _warnings;

    public FileManager(string path) => _path = path;

    public static List<Character> CreateDefaultCharacters() => new()
    {
        new Knight(1, "Sir Aldric", 120, 18, 12),
        new Archer(2, "Lyra Swiftarrow", 90, 24, 6),
        new Mage(3, "Elric the Wise", 80, 20, 5),
        new Knight(4, "Brienne", 130, 16, 14)
    };

    public List<Character> Load()
    {
        _warnings.Clear();

        if (!File.Exists(_path))
        {
            _warnings.Add("Character file not found. Default characters were created.");
            return UseDefaults();
        }

        try
        {
            string json = File.ReadAllText(_path);
            var loaded = JsonSerializer.Deserialize<List<Character>>(json, Options);

            if (loaded is null)
            {
                _warnings.Add("Character file was empty. Default characters were created.");
                return UseDefaults();
            }

            var valid = new List<Character>();
            foreach (var character in loaded)
            {
                if (character is null)
                {
                    _warnings.Add("Skipped an empty character entry.");
                }
                else if (!character.IsValid(out string error))
                {
                    _warnings.Add($"Skipped invalid character '{character.Name}': {error}");
                }
                else if (valid.Exists(c => c.Id == character.Id))
                {
                    _warnings.Add($"Skipped '{character.Name}': duplicate ID {character.Id}.");
                }
                else
                {
                    valid.Add(character);
                }
            }

            if (valid.Count == 0)
            {
                _warnings.Add("No valid characters were found. Default characters were created.");
                return UseDefaults();
            }

            return valid;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            _warnings.Add($"Character file is damaged ({ex.Message}). Default characters were created.");
            return UseDefaults();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _warnings.Add($"Character file could not be read ({ex.Message}). Default characters were created.");
            return CreateDefaultCharacters();
        }
    }

    public void Save(IEnumerable<Character> characters)
    {
        string? folder = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(folder))
            Directory.CreateDirectory(folder);

        string json = JsonSerializer.Serialize(characters.ToList(), Options);
        File.WriteAllText(_path, json);
    }

    private List<Character> UseDefaults()
    {
        var defaults = CreateDefaultCharacters();
        try
        {
            Save(defaults);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _warnings.Add($"Could not write the default file ({ex.Message}).");
        }
        return defaults;
    }
}
