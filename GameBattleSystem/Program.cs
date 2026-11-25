using GameBattleSystem;

try
{
    GameSystem.CreateDefault().Run();
}
catch (Exception ex)
{
    Console.WriteLine($"The game stopped unexpectedly: {ex.Message}");
}
