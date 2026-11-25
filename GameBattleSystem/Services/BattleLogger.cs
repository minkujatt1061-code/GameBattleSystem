using GameBattleSystem.Interfaces;

namespace GameBattleSystem.Services;

/// <summary>Appends timestamped battle actions to a text file. A logging failure never stops the game.</summary>
public class BattleLogger : IBattleLogger
{
    private readonly string _path;
    private bool _warned;

    public BattleLogger(string path) => _path = path;

    public void Log(string message)
    {
        try
        {
            string? folder = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(folder))
                Directory.CreateDirectory(folder);

            File.AppendAllText(_path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            if (!_warned)
            {
                Console.WriteLine($"[Warning] Could not write to the battle log: {ex.Message}");
                _warned = true;
            }
        }
    }
}
