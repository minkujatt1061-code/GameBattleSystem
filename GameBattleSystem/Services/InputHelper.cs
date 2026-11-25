namespace GameBattleSystem.Services;

/// <summary>Console input that keeps asking until the user types something valid.</summary>
public static class InputHelper
{
    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();
            if (line is null) throw new EndOfStreamException();

            if (int.TryParse(line.Trim(), out int value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Invalid input. Please enter a whole number from {min} to {max}.");
        }
    }

    public static string ReadText(string prompt, int maxLength = 20)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();
            if (line is null) throw new EndOfStreamException();

            line = line.Trim();
            if (line.Length > 0 && line.Length <= maxLength)
                return line;

            Console.WriteLine($"Invalid input. Enter text between 1 and {maxLength} characters.");
        }
    }
}
